using System.Collections.Generic;

namespace JUSToolkit.Formats
{
    /// <summary>
    /// Text Format for the Quiz.
    /// </summary>
    public class BinQuiz : Bin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinQuiz"/> class.
        /// </summary>
        public BinQuiz()
        {
            Pointers = new Queue<int>();
            FillPointers = new Queue<int>();
            Offsets = new Queue<int>();
        }

        /// <summary>
        /// Gets the Offsets where real pointers are.
        /// </summary>
        public Queue<int> Offsets { get; }

        /// <summary>
        /// Gets the Pointers.
        /// </summary>
        public Queue<int> Pointers { get; }

        /// <summary>
        /// Gets the FillPointers.
        /// </summary>
        public Queue<int> FillPointers { get; }

        /// <summary>
        /// Gets or sets the Unkown2 value.
        /// </summary>
        public int Unknown { get; set; }

        /// <summary>
        /// Gets or sets the Unkown2 value.
        /// </summary>
        public int Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the FirstPointer value .
        /// </summary>
        public int FirstPointer { get; set; }
    }
}
