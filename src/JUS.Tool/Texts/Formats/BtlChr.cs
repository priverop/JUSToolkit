using System;
using System.Text;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for chr_b_t.bin file.
    /// </summary>
    public class BtlChr : IFormat
    {
        /// <summary>
        /// Gets or sets the list of <see cref="BtlChrEntry" />.
        /// </summary>
        public List<BtlChrEntry> Entries { get; set; } = [];
    }
}
