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
    using System.Drawing;
    using Texim.Processing;
    using Texim;
    using System.Text;

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
                string dirToSave = args[2];
                string dataToInsert = null;

                // inputFilename - dirtosave - (dir/file to insert)

                if (args[0] != "-e"){
                    dataToInsert = args[3];
                }

                if (inputFileName == ".")
                {
                    ProcessDir(type, dirToSave, dataToInsert);
                }
                else
                {
                    log.Info("Identifying file " + inputFileName);

                    Node n = NodeFactory.FromFile(inputFileName);
                    ProcessFile(type, n, dirToSave, dataToInsert);
                }

                log.Info("Program completed.");

            }
        }
       
        private static void ProcessDir(string type, string dirToSave, string dataToInsert)
        {
            switch (type)
            {
                case "-exportdig":

                    Node digContainer = NodeFactory.FromDirectory(dataToInsert, "*.dig");
                    Node almtContainer = NodeFactory.FromDirectory(dataToInsert, "*.atm");

                    foreach (Node d in digContainer.Children)
                    {
                        Identify i = new Identify();
                        Format inputFormat = i.GetFormat(d);
                        Node dig = d;

                        // Compressed file
                        if (inputFormat.ToString() == FORMATPREFIX + "DSCP")
                        {
                            BinaryFormat binary = Utils.Lzss(new BinaryFormat(dig.Stream), "-d");
                            dig = new Node(dig.Name, binary);
                        }

                        Node atm = almtContainer.Children[Path.GetFileNameWithoutExtension(dig.Name) + ".atm"];
                        if (atm != null) {
                            ExportDig(dig, atm, dirToSave);
                        }
                    }
                    log.Info("Finished exporting");
                    break;
            }
        }

        private static void ProcessFile(string type, Node n, string dirToSave, string dataToInsert)
        {
            Identify i = new Identify();

            Format inputFormat = i.GetFormat(n);

            // Compressed file
            if (inputFormat.ToString() == FORMATPREFIX + "DSCP")
            {
                BinaryFormat binary = Utils.Lzss(new BinaryFormat(n.Stream), "-d");
                n = new Node(n.Name, binary);
                inputFormat = i.GetFormat(n);
            }

            log.Info("Format detected: " + inputFormat.ToString());

            switch (type)
            {// inputFilename - dirtosave - (dir/file to insert)
                case "-e":
                    Export(inputFormat.ToString(), n, dirToSave);
                    break;

                case "-i":
                    Import(inputFormat.ToString(), n, dirToSave, dataToInsert);
                    break;
            }
        }

        private static void ExportDig(Node nDIG, Node nATM, string outputPath)
        {
            log.Info("Exporting "+ nDIG.Name);

            Binary2DIG converterDig = new Binary2DIG();
            converterDig.IgnoreFirstTile = false;

            DIG dig = nDIG.Transform<BinaryFormat, DIG>(converterDig).GetFormatAs<DIG>();

            ALMT atm = nATM.Transform<Binary2ALMT, BinaryFormat, ALMT>().GetFormatAs<ALMT>();

            Bitmap img = atm.CreateBitmap(dig.Pixels,dig.Palette);

            string path = Path.Combine(outputPath, nDIG.Name + ".png");
            img.Save(path);

            log.Info("Saved into "+path);
        }

        private static void Export(string format, Node n, string outputPath){

            log.Info("Exporting...");

            switch (format)
            {
                case FORMATPREFIX + "BinTutorial":

                    n.Transform<BinaryFormat2BinTutorial, BinaryFormat, BinTutorial>()
                    .Transform<Bin2Po, BinTutorial, Po>()
                    .Transform<Po2Binary, Po, BinaryFormat>()
                    .Stream.WriteTo(Path.Combine(outputPath, n.Name + ".po"));

                    break;

                case FORMATPREFIX + "BinQuiz":

                    var quizs = n.Transform<Binary2BinQuiz, BinaryFormat, BinQuiz>()
                    .Transform<Quiz2Po, BinQuiz, NodeContainerFormat>();

                    foreach (Node po in quizs.Children) {
                        string outputFile = Path.Combine(outputPath, po.Name + ".po");
                        log.Info("Saving " + outputFile);
                        po.Transform<Po2Binary, Po, BinaryFormat>()
                        .Stream.WriteTo(outputFile);
                    }

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

            }

            log.Info("Finished exporting");

        }

        private static void Import(string format, Node n, string dirToSave, string dataToInsert)
        {
            log.Info("Importing...");

            switch (format)
            {
                case FORMATPREFIX + "ALAR.ALAR3":

                    // Alar original
                    Node original = n.Transform<BinaryFormat2Alar3, BinaryFormat, ALAR3>();

                    // Contenedor con los ficheros a insertar
                    var newAlar = NodeFactory.FromDirectory(dataToInsert, "*.*")
                        .Transform<Alar3ToNodes, NodeContainerFormat, ALAR3>();

                    // Modificamos el Alar original con los nuevos ficheros a insertar
                    original.GetFormatAs<ALAR3>().InsertModification(newAlar.GetFormatAs<ALAR3>()); 

                    original.Transform<BinaryFormat2Alar3, ALAR3, BinaryFormat>()
                        .Stream.WriteTo(Path.Combine(dirToSave, n.Name + "new.aar"));

                break;

                case FORMATPREFIX + "DIG":

                    DIG originalDig = n.Transform<Binary2DIG, BinaryFormat, DIG>().GetFormatAs<DIG>();

                    // Import the new PNG file
                    Bitmap newImage = (Bitmap)Image.FromFile(dataToInsert);
                    var quantization = new FixedPaletteQuantization(originalDig.Palette.GetPalette(0));
                    Texim.ImageConverter importer = new Texim.ImageConverter {
                        Format = ColorFormat.Indexed_4bpp,
                        PixelEncoding = PixelEncoding.HorizontalTiles,
                        Quantization = quantization
                    };

                    (Palette _, PixelArray pixelInfo) = importer.Convert(newImage);

                    originalDig.Pixels = pixelInfo;

                    BinaryFormat b = originalDig.ConvertWith<Binary2DIG, DIG, BinaryFormat>();

                    Utils.Lzss(b, "-evn").Stream.WriteTo(Path.Combine(dirToSave, n.Name + "evn.dig"));

                    break;
            }
            log.Info("Finished importing");
        }

        private static void ShowUsage()
        {
            log.Info("Usage: JUSToolkit.exe -e <fileToExtract> <dirToSave>");
            log.Info("Usage: JUSToolkit.exe -iDir <inputFileName> <dirToSave> <dirWithFilesToInsert>");
            log.Info("Usage: JUSToolkit.exe -i <inputFileName> <dirToSave> <fileToInsert>");
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
