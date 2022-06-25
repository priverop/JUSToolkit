using System.Collections.Generic;

namespace JUSToolkit.Texts
{
    /// <summary>
    /// Text Format of the Tutorials of the Game.
    /// </summary>
    public class BinTutorial : Bin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinTutorial"/> class.
        /// </summary>
        public BinTutorial()
        {
            Pointers = new Dictionary<int, int>();
            FillPointers = new Queue<int>();
        }

        /// <summary>
        /// Gets the Pointers of the File.
        /// </summary>
        public Dictionary<int, int> Pointers { get; } // Pointer - Offset

        /// <summary>
        /// Gets the Fill Pointers of the File.
        /// </summary>
        public Queue<int> FillPointers { get; }

        /// <summary>
        /// Gets or sets the First Pointer of the first Text.
        /// </summary>
        public int FirstPointer { get; set; }
    }
}
