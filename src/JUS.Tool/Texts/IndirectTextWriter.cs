using System.Collections.Generic;

namespace JUSToolkit.Texts
{
    /// <summary>
    /// Class with utilities to work with Jus text files with absolute pointers.
    /// </summary>
    public class IndirectTextWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndirectTextWriter"/> class.
        /// </summary>
        /// <param name="startingOffset">Where the pointer section ends.</param>
        public IndirectTextWriter(int startingOffset)
        {
            TextOffsets = new Dictionary<string, int>();
            Strings = new List<string>();
            CurrentOffset = startingOffset;
        }

        /// <summary>
        /// Gets or sets the texts and its pointers.
        /// </summary>
        public Dictionary<string, int> TextOffsets { get; set; }

        /// <summary>
        /// Gets or sets the list of sentences / entries.
        /// </summary>
        public List<string> Strings { get; set; }

        /// <summary>
        /// Gets or sets the current pointer.
        /// </summary>
        public int CurrentOffset { get; set; }
    }
}
