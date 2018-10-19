namespace JUSToolkit
{
    using System;

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
