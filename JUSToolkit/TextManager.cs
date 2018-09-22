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
        public List<int> Pointers { get; }
        public List<string> Text { get; }

        public TextManager()
        {
            this.Pointers = new List<int>();
            this.Text = new List<string>();
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
                long pointerTableSize = firstPointer - currentPosition;
                fileToExtractStream.Position = currentPosition;

                // Read all the pointers

                while (fileToExtractStream.Position != firstPointer)
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

    }
}
