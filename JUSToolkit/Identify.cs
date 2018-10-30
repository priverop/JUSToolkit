
namespace JUSToolkit
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using System.Collections.Generic;
    using Formats;
    using System.IO;

    /// <summary>
    /// Identify allow us to Identify which Format are we entering to the program.
    /// It reads the extension and returns the Format of the file.
    /// </summary>
    public class Identify
    {
        public Dictionary<String, Delegate> extensionDictionary { get; set; }
        public Dictionary<String, Format> binDictionary { get; set; }
        public Dictionary<int, Format> alarDictionary { get;set;}

        public Identify(){

            extensionDictionary = new Dictionary<String, Delegate>
            {
                { ".aar", new Func<String, Format>(GetAlarFormat) },
                { ".bin", new Func<String, Format>(GetBinFormat) },
            };

            binDictionary = new Dictionary<String, Format>
            {
                { "TUTORIAL", new BINTUTORIAL() },
                { "FILENAME", new BINFILENAME() },
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
        public Format GetFormat(String filename)
        {
            String extension = Path.GetExtension(filename);

            if(extensionDictionary.ContainsKey(extension)){
                return (Format)extensionDictionary[extension].DynamicInvoke(filename);
            }
            else{
                throw new System.ArgumentException("Extension is not known.", extension);
            }
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

        public Format GetBinFormat(String filename)
        {
            using (DataStream fileToReadStream = new DataStream(filename, FileOpenMode.Read))
            {
                DataReader fileToReadReader = new DataReader(fileToReadStream);

                int firstPointer = fileToReadReader.ReadInt32();
                int secondPointer = fileToReadReader.ReadInt32();

                string fileCase = PrepareCases(firstPointer, secondPointer);

                return binDictionary[fileCase];
            }
        }

        private static string PrepareCases(int firstPointer, int secondPointer)
        {
            if (firstPointer > secondPointer)
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
         * Si es DSCP -> Descomprimimos LZSS offset 4 -> ALAR -> Leemos tipo.
         * 
         * Tipo -> 02 | 03.
         * 
         * Si es 02 -> Format ALAR2.
         * Si es 03 -> Format ALAR3.
         * 
         * Dudas: ¿Format DSCP?
         * 
         */

        public Format GetAlarFormat(String filename)
        {
            using (DataStream fileToReadStream = new DataStream(filename, FileOpenMode.Read))
            {
                DataReader fileToReadReader = new DataReader(fileToReadStream);

                string magic = fileToReadReader.ReadInt32().ToString();
                byte type = 00;

                if (magic == "ALAR")
                    type = fileToReadReader.ReadByte();
                else if (magic == "DSCP"){
                    // Descomprimir
                    // Nuevo reader y demás seguramente
                    type = fileToReadReader.ReadByte();
                }
                else{
                    throw new System.ArgumentException("Magic is not known.", magic);
                }

                return alarDictionary[type];
            }
        }

    }
}