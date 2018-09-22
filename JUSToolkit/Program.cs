namespace JUSToolkit
{
    using System;

    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length > 3 && args.Length < 2)
            {
                Console.Write("Wrong arguments.");
                showUsage();
            }
            else
            {
                TextManager tm = new TextManager();

                switch (args[0])
                {
                    case "-extractText":
                        tm.LoadFile(args[1]);
                        tm.ExportPO();
                        break;

                    case "-insertText":
                        tm.LoadFile(args[1]);
                        //tm.ImportPO(args[2]);
                        //tm.ExportBin();
                        break;

                    default:
                        Console.WriteLine("Wrong arguments.");
                        showUsage();
                        break;
                }
            }
        }

        private static void showUsage()
        {
            Console.WriteLine("Usage: JUSToolkit.exe -extractText <fileToExtract>");
            Console.WriteLine("Usage: JUSToolkit.exe -insertText <originalFile> <po>");
        }

        private static void showCredits()
        {
            Console.WriteLine("=========================");
            Console.WriteLine("== JUS TOOLKIT by Nex ==");
            Console.WriteLine("=========================");
        }
    }
}
