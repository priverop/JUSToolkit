using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for simple bin files.
    /// </summary>
    public class SimpleBin : IFormat
    {
        /// <summary>
        /// Size of a <see cref="SimpleBin"/> entry.
        /// </summary>
        public static readonly int EntrySize = 0x04;

        /// <summary>
        /// Gets or sets the list of text entries.
        /// </summary>
        public List<string> TextEntries { get; set; } = [];
    }
}
