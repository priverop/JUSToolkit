
namespace JUSToolkit
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using System.Collections.Generic;
    using Formats;
    using System.IO;
    using log4net;
    using System.Text;
    using Yarhl.FileSystem;
    using JUSToolkit.Formats.ALAR;

    /// <summary>
    /// Identify allow us to Identify which IFormat are we entering to the program.
    /// It reads the extension and returns the IFormat of the file.
    /// </summary>
    public class Identify
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Identify));

        public Dictionary<String, Delegate> extensionDictionary { get; set; }
        public Dictionary<String, IFormat> binDictionary { get; set; }
        public Dictionary<int, IFormat> alarDictionary { get;set;}

        public Identify(){

            extensionDictionary = new Dictionary<String, Delegate>
            {
                { ".aar", new Func<Node, IFormat>(GetAlarFormat) },
                { ".bin", new Func<Node, IFormat>(GetBinFormat) },
                { ".dig", new Func<Node, IFormat>(GetDigFormat) },
            };

            binDictionary = new Dictionary<String, IFormat>
            {
                { "TUTORIAL", new BinTutorial() },
                { "FILENAME", new BinFilename() },
                { "QUIZ", new BinQuiz() },
            };

            alarDictionary = new Dictionary<int, IFormat>
            {
                { 02, new ALAR2() },
                { 03, new ALAR3() },
            };
        }

        /// <summary>
        /// Gets the IFormat of the file passed.
        /// </summary>
        /// <returns>The format of the file passed by argument.</returns>
        public IFormat GetFormat(Node n)
        {
            String extension = Path.GetExtension(n.Name);

            log.Info("Extension: " + extension);

            if(!extensionDictionary.ContainsKey(extension)){
                throw new System.ArgumentException("Extension is not known.", extension);
            }

            if(IsCompressed(n)){
                log.Info("Compressed file.");
                return (IFormat)new DSCP();
            }
            else{
                log.Info("Not compressed");
            }

            return (IFormat)extensionDictionary[extension].DynamicInvoke(n);
            
        }

        private bool IsCompressed(Node node)
        {

            DataReader reader = new DataReader(node.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };
            reader.Stream.Position = 0;
            return reader.ReadString(4) == "DSCP" ? true : false;

        }

        /*
        * BIN:
        * 
        * Extensión .bin
        *
        * Necesitamos los diferentes identificadores para la cabecera con los dos primeros punteros.
        * 
        * Switch.
        * 
        * Tendríamos un IFormat por cada tipo.
        */

        public IFormat GetBinFormat(Node node)
        {

            DataReader fileToReadReader = new DataReader(node.Stream);
            fileToReadReader.Stream.Position = 0;

            int firstPointer = fileToReadReader.ReadInt32();
            int secondPointer = fileToReadReader.ReadInt32();

            string fileCase = PrepareCases(firstPointer, secondPointer);

            return binDictionary[fileCase];

        }

        private static string PrepareCases(int firstPointer, int secondPointer)
        {
            if (secondPointer == 255) {
                return "QUIZ";
            }
            else if (firstPointer > secondPointer)
                return "TUTORIAL";
            else
                return "FILENAME";
        }

        /*
         * ALAR:
         * 
         * Extensión .aar
         * Magic ALAR | DSCP
         * 
         * Si es ALAR -> Leemos tipo.
         * 
         * Tipo -> 02 | 03.
         * 
         * Si es 02 -> IFormat ALAR2.
         * Si es 03 -> IFormat ALAR3.
         * 
         */

        public IFormat GetAlarFormat(Node node)
        {
            DataReader fileToReadReader = new DataReader(node.Stream);

            fileToReadReader.Stream.Position = 0;

            byte[] magicBytes = fileToReadReader.ReadBytes(4);

            string magic = new String(Encoding.ASCII.GetChars(magicBytes));

            byte type = 00;

            if (magic == "ALAR")
            {
                type = fileToReadReader.ReadByte();
            }
            else{
                throw new System.ArgumentException("Magic is not known.", magic);
            }

            return alarDictionary[type];

        }

        public IFormat GetDigFormat(Node node){

            DataReader reader = new DataReader(node.Stream);

            reader.Stream.Position = 0;

            string magic = reader.ReadString(4);

            if (magic == "DSIG")
            {
                return new DIG();
            }
            else
            {
                throw new System.ArgumentException("Magic is not known.", magic);
            }


        }

    }
}