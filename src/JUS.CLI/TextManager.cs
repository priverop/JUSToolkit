namespace JUSToolkit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Yarhl.IO;
    using Yarhl.Media.Text;
    using Yarhl.FileFormat;

    public class TextManager
    {
        public string FileName { get; set; }

        public Dictionary<int, int> Pointers { get; } // Pointer - Offset
        public Queue<int> FillPointers { get; }
        public int FirstPointer { get; set; } 
        public Dictionary<string, int> Text { get; } // String - Pointer

        // Import PO
        public Queue<string> newText { get; set; } // Translated text
        public Queue<int> newPointers { get; set; } // New pointers

        // Write
        private Dictionary<Char, Char> spanishChars;

        public TextManager()
        {
            this.Pointers = new Dictionary<int, int>();
            this.Text = new Dictionary<string, int>();
            this.FillPointers = new Queue<int>();
            this.spanishChars = new Dictionary<Char, Char>
            {
                { '¡', '{' },
                { 'ó', '\\' },
                { 'ú', '^' },
                { 'á', '*' },
                { 'é', '/' },
                { '¿', '@' },
                { 'í', '$' },
                { 'ñ', '}' }
            };
        }

        public void LoadFile(string fileToExtractName)
        {
            using (DataStream fileToExtractStream = new DataStream(fileToExtractName, FileOpenMode.Read))
            {
                DataReader fileToExtractReader = new DataReader(fileToExtractStream)
                {
                    DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEnconding("ascii")
                };

                this.FileName = fileToExtractName;

                this.FirstPointer = fileToExtractReader.ReadInt32();

                long currentPosition = fileToExtractStream.Position;
                int secondPointer = fileToExtractReader.ReadInt32();
                fileToExtractStream.Position = currentPosition;

                // Tutorials start with a big pointer for the first string
                if(this.FirstPointer > secondPointer){

                    // Read Strings and calculate pointers

                    fileToExtractStream.Position = this.FirstPointer;

                    int actualPointer = 0;
                    while (!fileToExtractReader.Stream.EndOfStream)
                    {
                        string sentence = fileToExtractReader.ReadString();
                        actualPointer += sentence.Length +1 ; // \0 char 
                        this.Text.Add(sentence, actualPointer);
                    }

                    fileToExtractStream.Position = 4; // skip first pointer

                    // Read pointers
                    while (fileToExtractStream.Position < this.FirstPointer)
                    {
                        int offset = (int)fileToExtractStream.Position;
                        int pointer = fileToExtractReader.ReadInt32();

                        if (this.Text.ContainsValue(pointer) && !this.Pointers.ContainsKey(pointer))
                            this.Pointers.Add(pointer, offset);
                        else
                            this.FillPointers.Enqueue(pointer);
                    }
                }
            }
        }

        public void ExportPO(){

            Po poExport = new Po
            {
                Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es")
                {
                    LanguageTeam = "TranScene",
                }
            };

            int i = 0;
            foreach (KeyValuePair<string, int> entry in this.Text)
            {
                string sentence = entry.Key;
                if (string.IsNullOrEmpty(sentence))
                    sentence = "<!empty>";
                poExport.Add(new PoEntry(sentence) { Context = i.ToString() });
            }

            poExport.ConvertTo<BinaryFormat>().Stream.WriteTo(this.FileName + ".po");
        }

        public void ImportPO(string poFileName)
        {
            DataStream inputPO = new DataStream(poFileName, FileOpenMode.Read);
            BinaryFormat binaryFile = new BinaryFormat(inputPO);
            Po newPO = binaryFile.ConvertTo<Po>();
            inputPO.Dispose();

            this.newText = new Queue<string>();
            this.newPointers = new Queue<int>();

            int longCounter = 0;

            foreach (var entry in newPO.Entries)
            {
                string sentence = string.IsNullOrEmpty(entry.Translated) ?
                    entry.Original : entry.Translated;

                if (sentence == "<!empty>")
                    sentence = string.Empty;

                if(this.CheckCorrectLong(sentence)){
                    sentence = this.ReplaceSpecialChars(sentence);
                    longCounter += sentence.Length + 1; // byte \0
                    this.newText.Enqueue(sentence);
                    this.newPointers.Enqueue(longCounter);
                }
                else
                    Console.WriteLine("Tamaño de frase excedido, máximo 36 caracteres: "+sentence);
            }
        }

        public void ExportBin()
        {
            using (DataStream exportedFileStream = new DataStream(this.FileName + "_new", FileOpenMode.Write))
            {
                DataWriter exportedFileWriter = new DataWriter(exportedFileStream);

                exportedFileWriter.Write(this.FirstPointer);

                int offset = 4;

                for (; offset < this.FirstPointer; offset += 4)
                {
                    if (this.Pointers.ContainsValue(offset))
                    {
                        exportedFileWriter.Write(this.newPointers.Dequeue());
                    }
                    else
                        exportedFileWriter.Write(this.FillPointers.Dequeue());
                }

                exportedFileStream.Position = this.FirstPointer;

                while (this.newText.Count > 0)
                {
                    exportedFileWriter.Write(this.ReplaceSpanishChars(this.newText.Dequeue()));
                }

            }
        }

        private string ReplaceSpecialChars(string sentence){
            return sentence.Replace('…', '@').Replace("@", "...");
        }

        private string ReplaceSpanishChars(string sentence)
        {
            return sentence.Replace('í', this.spanishChars['í'])
                            .Replace('ó', this.spanishChars['ó'])
                            .Replace('ú', this.spanishChars['ú'])
                            .Replace('á', this.spanishChars['á'])
                            .Replace('é', this.spanishChars['é'])
                            .Replace('¿', this.spanishChars['¿'])
                            .Replace('¡', this.spanishChars['¡'])
                            .Replace('ñ', this.spanishChars['ñ']);

        }

        private bool CheckCorrectLong(string sentence){
            return sentence.IndexOf("\n", StringComparison.CurrentCulture) <= 36;
        }

    }
}
