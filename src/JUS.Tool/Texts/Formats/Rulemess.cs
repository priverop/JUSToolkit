using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for Rulemess.bin file.
    /// </summary>
    public class Rulemess : IFormat
    {
        /// <summary>
        /// Gets or sets the list of <see cref="RulemessEntry" />.
        /// </summary>
        public List<RulemessEntry> Entries { get; set; } = [];
    }
}
