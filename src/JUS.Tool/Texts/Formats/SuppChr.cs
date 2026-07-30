using System;
using System.Text;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for chr_s_t.bin file.
    /// </summary>
    public class SuppChr : IFormat
    {
        /// <summary>
        /// Gets or sets the list of <see cref="SuppChrEntry" />.
        /// </summary>
        public List<SuppChrEntry> Entries { get; set; } = [];
    }
}
