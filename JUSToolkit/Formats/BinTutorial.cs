namespace JUSToolkit.Formats
{
    using System;
    using Yarhl.FileFormat;
    using System.Collections.Generic;

    public class BinTutorial : Bin
    {
        public Dictionary<int, int> Pointers { get; } // Pointer - Offset

        public Queue<int> FillPointers { get; }

        public int FirstPointer { get; set; }

        public BinTutorial(){
            Pointers = new Dictionary<int, int>();
            FillPointers = new Queue<int>();
        }

    }
}
