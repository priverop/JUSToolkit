
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
    /// Identify allow us to Identify which Format are we entering to the program.
    /// It reads the extension and returns the Format of the file.
    /// </summary>
    public class Identify
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Identify));

        public Dictionary<String, Delegate> extensionDictionary { get; set; }
        public Dictionary<String, Format> binDictionary { get; set; }
        public Dictionary<int, Format> alarDictionary { get;set;}

        public Identify(){

            extensionDictionary = new Dictionary<String, Delegate>
            {
                { ".aar", new Func<Node, Format>(GetAlarFormat) },
                { ".bin", new Func<Node, Format>(GetBinFormat) },
                { ".dig", new Func<Node, Format>(GetDigFormat) },
            };

            binDictionary = new Dictionary<String, Format>
            {
                { "TUTORIAL", new BinTutorial() },
                { "FILENAME", new BinInfoTitle() },
                { "QUIZ", new BinQuiz() },
            };

            alarDictionary = new Dictionary<int, Format>
            {
                { 02, new ALAR2() },
                { 03, new ALAR3() },
            };
        }

        /// <summary>
        /// Gets the Format of the file passed.
        /// </summary>
        /// <returns>The format of the file passed by argument.</returns>
        public Format GetFormat(Node n)
        {
            String extension = Path.GetExtension(n.Name);

            log.Info("Extension: " + extension);

            if(!extensionDictionary.ContainsKey(extension)){
                throw new System.ArgumentException("Extension is not known.", extension);
            }

            if(IsCompressed(n)){
                log.Info("Compressed file.");
                return (Format)new DSCP();
            }
            else{
                log.Info("Not compressed");
            }

            return (Format)extensionDictionary[extension].DynamicInvoke(n);
            
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
        * Tendríamos un Format por cada tipo.
        */

        public Format GetBinFormat(Node node)
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
         * Si es 02 -> Format ALAR2.
         * Si es 03 -> Format ALAR3.
         * 
         */

        public Format GetAlarFormat(Node node)
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

        public Format GetDigFormat(Node node){

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