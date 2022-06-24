using System.Collections.Generic;

namespace JUSToolkit.Formats
{
    public class BinFilename : Bin
    {
        public Queue<int> Offsets { get; } // Offsets where real pointers are
        public Queue<int> Pointers { get; }
        public Queue<int> FillPointers { get; } // Trash pointers
        public int FirstPointer { get; set; }

        public BinFilename()
        {
            Pointers = new Queue<int>();
            FillPointers = new Queue<int>();
            Offsets = new Queue<int>();
        }
    }
}
