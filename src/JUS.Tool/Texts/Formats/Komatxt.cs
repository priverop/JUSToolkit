using System;
using System.Text;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for Komatxt.bin file.
    /// </summary>
    public class Komatxt : IFormat
    {
        /// <summary>
        /// Gets or sets the list of <see cref="KomatxtEntry" />.
        /// </summary>
        public List<KomatxtEntry> Entries { get; set; } = [];
    }
}
