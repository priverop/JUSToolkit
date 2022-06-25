using System.Collections.Generic;

namespace JUSToolkit.Formats
{
    public class BinQuiz : Bin
    {
        public Queue<int> Offsets { get; } // Offsets where real pointers are
        public Queue<int> Pointers { get; }

        public Queue<int> FillPointers { get; }

        public int Uknown { get; set; }
        public int Uknown2 { get; set; }
        public int FirstPointer { get; set; }

        public BinQuiz()
        {
            Pointers = new Queue<int>();
            FillPointers = new Queue<int>();
            Offsets = new Queue<int>();
        }
    }
}
