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
        public Byte[] Header { get; set; }
        private const int HEADERSIZE = 24;
        public List<int> Pointers { get; }
        public List<string> Text { get; }

        // Import PO
        private List<string> newText;
        private List<int> newPointers;

        // Write
        private Dictionary<Char, Char> spanishChars;

        public TextManager()
        {
            this.Pointers = new List<int>();
            this.Text = new List<string>();
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
            Header = new Byte[HEADERSIZE];
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

                long currentPosition = fileToExtractStream.Position;
                int firstPointer = fileToExtractReader.ReadInt32();
                int secondPointer = fileToExtractReader.ReadInt32();
                fileToExtractStream.Position = currentPosition;

                // Read header
                this.Header = fileToExtractReader.ReadBytes(HEADERSIZE);

                // Read pointers
                while (fileToExtractStream.Position < firstPointer)
                {
                    int pointer = fileToExtractReader.ReadInt32();
                    this.Pointers.Add(pointer);
                }

                if(firstPointer > secondPointer){
                    fileToExtractStream.Position = firstPointer;

                    while(!fileToExtractReader.Stream.EndOfStream){
                        this.Text.Add(fileToExtractReader.ReadString());
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

            for (int i = 0; i < this.Text.Count; i++)
            {
                string sentence = this.Text[i];
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

            this.newText = new List<string>();
            this.newPointers = new List<int>();

            int longCounter = 0;

            foreach (var entry in newPO.Entries)
            {
                string sentence = string.IsNullOrEmpty(entry.Translated) ?
                    entry.Original : entry.Translated;
                if (sentence == "<!empty>")
                    sentence = string.Empty;
                if(this.CheckCorrectLong(sentence)){
                    sentence = this.ReplaceSpecialChars(sentence);
                    this.newText.Add(sentence);

                    longCounter += sentence.Length + 1; // byte \0
                    this.newPointers.Add(longCounter);
                }
                else{
                    Console.WriteLine("Tamaño de frase excedido, máximo 36 caracteres: "+sentence);
                }


            }
        }

        public void ExportBin()
        {
            using (DataStream exportedFileStream = new DataStream(this.FileName + "_new", FileOpenMode.Write))
            {
                DataWriter exportedFileWriter = new DataWriter(exportedFileStream);

                exportedFileWriter.Write(Header);

                int pointerCount = 0;

                for (int i = 0; i < this.Pointers.Count; i++)
                {
                    if (this.Pointers[i] > 0x18 && this.Pointers[i] < 0x010000 && this.Pointers[i] != 0xB4 && this.Pointers[i] != 0x78 && this.Pointers[i] != 0x64)
                    {
                        exportedFileWriter.Write(this.newPointers[pointerCount]);
                        //Console.WriteLine(pointerCount + " - punteroNuevo "+ this.newPointers[pointerCount]);
                        pointerCount++;
                    }
                    else{
                        exportedFileWriter.Write(this.Pointers[i]);
                        //Console.WriteLine(i + " - puntero " + this.Pointers[i]);
                    }

                }

                foreach (var sentence in this.newText)
                {
                    exportedFileWriter.Write(this.ReplaceSpanishChars(sentence));
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
