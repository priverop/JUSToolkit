using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
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
        /// Initializes a new instance of the <see cref="SimpleBin"/> class.
        /// </summary>
        public SimpleBin()
        {
            TextEntries = new List<string>();
        }

        /// <summary>
        /// Gets or sets the list of text entries.
        /// </summary>
        public List<string> TextEntries { get; set; }
    }
}
