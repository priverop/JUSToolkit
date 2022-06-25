using System.Collections.Generic;

namespace JUSToolkit.Formats
{
    /// <summary>
    /// Text Format for Filenames bin.
    /// </summary>
    public class BinFilename : Bin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinFilename"/> class.
        /// </summary>
        public BinFilename()
        {
            Pointers = new Queue<int>();
            FillPointers = new Queue<int>();
            Offsets = new Queue<int>();
        }

        /// <summary>
        /// Gets the Offsets of the File.
        /// </summary>
        public Queue<int> Offsets { get; } // Offsets where real pointers are

        /// <summary>
        /// Gets the Pointers of the File.
        /// </summary>
        public Queue<int> Pointers { get; }

        /// <summary>
        /// Gets the FillPointers (trash pointers) of the File.
        /// </summary>
        public Queue<int> FillPointers { get; }

        /// <summary>
        /// Gets or sets the First pointer of the text.
        /// </summary>
        public int FirstPointer { get; set; }
    }
}
