namespace JUSToolkit
{
    using System;
    using Yarhl.FileFormat;

    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Write("Wrong arguments.");
                showUsage();
            }
            else
            {
                /*
                 * Identify -> get Format -> call Converter
                 */

                Identify i = new Identify();

                // Aquí casi mejor usar Node?

                Format input = i.GetFormat(args[1]);

                // Switch para los conversores
                // input.convertwith...
            }
        }

        private static void showUsage()
        {
            Console.WriteLine("Usage: JUSToolkit.exe <fileToExtract>");
        }

        private static void showCredits()
        {
            Console.WriteLine("=========================");
            Console.WriteLine("== JUS TOOLKIT by Nex ==");
            Console.WriteLine("=========================");
        }
    }
}
