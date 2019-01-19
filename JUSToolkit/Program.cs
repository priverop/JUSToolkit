namespace JUSToolkit
{
    using System;
    using Yarhl.FileFormat;
    using log4net;
    using log4net.Config;
    using Formats;
    using Converters.Bin;
    using Converters.Alar;
    using Converters.Images;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;
    using System.IO;
    using JUSToolkit.Formats.ALAR;

    class MainClass
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(MainClass));
        private const String FORMATPREFIX = "JUSToolkit.Formats.";

        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            if (args.Length < 3 || args.Length > 4)
            {
                log.Error("Wrong arguments.");
                ShowUsage();
            }
            else
            {
                ShowCredits();

                string type = args[0];
                string inputFileName = args[1];
                string outputPath = args[2];
                string dirToSave = null;

                if (args[0] == "-i"){
                    dirToSave = args[3];
                }

                Identify i = new Identify();

                log.Info("Identifying file " + inputFileName);

                Node n = NodeFactory.FromFile(inputFileName);

                Format inputFormat = i.GetFormat(n);

                // Compressed file
                if(inputFormat.ToString() == FORMATPREFIX + "DSCP"){
                    n = Utils.DecompressLzss(n);
                    inputFormat = i.GetFormat(n);
                }

                log.Info("Format detected: " + inputFormat.ToString());

                switch(type){
                    case "-e":
                        Export(inputFormat.ToString(), n, outputPath, inputFileName);
                    break;

                    case "-i":
                        Import(inputFormat.ToString(), n, outputPath, inputFileName, dirToSave);
                    break;
                }

                log.Info("Program completed.");

            }
        }

        private static void Export(string format, Node n, string outputPath, string inputFileName){

            log.Info("Exporting...");

            switch (format)
            {
                case FORMATPREFIX + "BinTutorial":

                    n.Transform<BinaryFormat2BinTutorial, BinaryFormat, BinTutorial>()
                    .Transform<Bin2Po, BinTutorial, Po>()
                    .Transform<Po2Binary, Po, BinaryFormat>()
                    .Stream.WriteTo(outputPath + Path.PathSeparator + inputFileName + ".po");

                    break;

                case FORMATPREFIX + "BinQuiz":

                    n.Transform<Binary2BinQuiz, BinaryFormat, BinQuiz>()
                    .Transform<Bin2Po, BinQuiz, Po>()
                    .Transform<Po2Binary, Po, BinaryFormat>()
                    .Stream.WriteTo(outputPath + Path.PathSeparator + inputFileName + ".po");

                    break;

                case FORMATPREFIX + "ALAR.ALAR3":

                    var folder = n.Transform<BinaryFormat2Alar3, BinaryFormat, ALAR3>()
                        .Transform<Alar3ToNodes, ALAR3, NodeContainerFormat>();

                    SaveToDir(folder, outputPath);

                    break;

                case FORMATPREFIX + "ALAR.ALAR2":

                    var root = n.Transform<BinaryFormat2Alar2, BinaryFormat, ALAR2>()
                        .Transform<Alar2ToNodes, ALAR2, NodeContainerFormat>();

                    SaveToDir(root, outputPath);

                    break;

                case FORMATPREFIX + "DIG":

                    DIG dig = n.Transform<Binary2DIG, BinaryFormat, DIG>().GetFormatAs<DIG>();

                    var img = dig.Pixels.CreateBitmap(dig.Palette, 0);

                    img.Save(inputFileName + ".png");

                    break;

            }

            log.Info("Finished exporting");

        }

        private static void Import(string format, Node n, string dirWithFilesToInsert, string inputFileName, string dirToSave)
        {
            log.Info("Importing...");

            switch (format)
            {
                case FORMATPREFIX + "ALAR.ALAR3":

                    // Alar original
                    Node original = n.Transform<BinaryFormat2Alar3, BinaryFormat, ALAR3>();

                    // Contenedor con los ficheros a insertar
                    var newAlar = NodeFactory.FromDirectory(dirWithFilesToInsert)//*** We should exclude .DS files and thumb
                        .Transform<Alar3ToNodes, NodeContainerFormat, ALAR3>();

                    // Modificamos el Alar original con los nuevos ficheros a insertar
                    original.GetFormatAs<ALAR3>().InsertModification(newAlar.GetFormatAs<ALAR3>()); 

                    original.Transform<BinaryFormat2Alar3, ALAR3, BinaryFormat>()
                        .Stream.WriteTo(dirToSave + Path.PathSeparator + inputFileName + ".aar");

                break;
            }
            log.Info("Finished importing");
        }

        private static void ShowUsage()
        {
            log.Info("Usage: JUSToolkit.exe -e <fileToExtract> <dirToExtractTo>");
            log.Info("Usage: JUSToolkit.exe -i <originalFile> <dirWithFilesToInsert> <dirToSave>");
        }

        private static void ShowCredits()
        {
            log.Info("=========================");
            log.Info("== JUS TOOLKIT by Nex ==");
            log.Info("== Using YARHL by Pleonex ==");
            log.Info("=========================");
        }

        private static void SaveToDir(Node folder, string output){
            Directory.CreateDirectory(output);
            foreach (var child in folder.Children)
            {
                string outputFile = Path.Combine(output, child.Name);
                log.Info("Saving " + outputFile);
                child.GetFormatAs<BinaryFormat>().Stream.WriteTo(outputFile);
            }

            log.Info("Saved in " + output);
        }
    }
}
