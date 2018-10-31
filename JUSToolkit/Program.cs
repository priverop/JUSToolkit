namespace JUSToolkit
{
    using System;
    using Yarhl.FileFormat;
    using log4net;
    using log4net.Config;

    class MainClass
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainClass));

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

                // Aquí casi mejor usar Node?

                Format input = i.GetFormat(args[0]);

                log.Info("Format detected: " + input.ToString());

                // Switch para los conversores

                // input.convertwith...
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
