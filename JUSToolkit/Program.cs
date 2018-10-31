namespace JUSToolkit
{
    using System;
    using Yarhl.FileFormat;
    using log4net;
    using log4net.Config;
    using Formats;
    using Converters.Bin;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    class MainClass
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(MainClass));
        private const String FORMATPREFIX = "JUSToolkit.Formats.";

        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            if (args.Length != 1)
            {
                log.Error("Wrong arguments.");
                showUsage();
            }
            else
            {
                showCredits();
                /*
                 * Identify -> get Format -> call Converter
                 */

                Identify i = new Identify();

                log.Info("Identifying file " + args[0]);

                Format inputFormat = i.GetFormat(args[0]);

                log.Info("Format detected: " + inputFormat.ToString());

                Node n = NodeFactory.FromFile(args[0]);

                switch (inputFormat.ToString())
                {
                    case FORMATPREFIX + "BinTutorial":
                        n.Transform<BinaryFormat2BinTutorial, BinaryFormat, BinTutorial>()
                        .Transform<Bin2Po, BinTutorial, Po>()
                        .Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(args[0] + ".po");
                        break;
                }

                log.Info("Program completed.");



            }
        }

        private static void showUsage()
        {
            log.Info("Usage: JUSToolkit.exe <fileToExtract>");
        }

        private static void showCredits()
        {
            log.Info("=========================");
            log.Info("== JUS TOOLKIT by Nex ==");
            log.Info("=========================");
        }
    }
}
