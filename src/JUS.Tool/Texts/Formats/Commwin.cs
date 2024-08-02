using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for Commwin files.
    /// </summary>
    public class Commwin : IFormat
    {
        /// <summary>
        /// Size of a <see cref="Commwin"/> entry.
        /// </summary>
        public static readonly int EntrySize = 0x04;

        /// <summary>
        /// Initializes a new instance of the <see cref="Commwin"/> class.
        /// </summary>
        public Commwin()
        {
            TextEntries = new List<string>();
            UnknownEntries = new List<int>();
        }

        /// <summary>
        /// Gets or sets the list of Unknown entries.
        /// </summary>
        public List<int> UnknownEntries { get; set; }

        /// <summary>
        /// Gets or sets the list of text entries.
        /// </summary>
        public List<string> TextEntries { get; set; }
    }
}
