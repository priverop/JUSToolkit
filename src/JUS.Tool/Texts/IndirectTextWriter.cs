using System.Collections.Generic;

namespace JUSToolkit.Texts
{
    /// <summary>
    /// Class with utilities to work with Jus text files with relative pointers.
    /// </summary>
    public class JusIndirectText
    {
        /// <summary>
        /// Texts and its pointers.
        /// </summary>
        public Dictionary<string, int> TextOffsets { get; set; }

        /// <summary>
        /// List of sentences / entries.
        /// </summary>
        public List<string> Strings { get; set; }

        /// <summary>
        /// Current pointer.
        /// </summary>
        public int CurrentOffset { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JusIndirectText"/> class.
        /// </summary>
        public JusIndirectText(int startingOffset)
        {
            TextOffsets = new Dictionary<string, int>();
            Strings = new List<string>();
            CurrentOffset = startingOffset;
        }
    }
}
