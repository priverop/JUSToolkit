using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for Bgm.bin file.
    /// </summary>
    public class Bgm : IFormat
    {
        /// <summary>
        /// Gets or sets the number of entries in <see cref="Entries"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="BgmEntry" />.
        /// </summary>
        public List<BgmEntry> Entries { get; set; } = [];
    }
}
