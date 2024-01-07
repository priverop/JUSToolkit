using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for Rulemess.bin file.
    /// </summary>
    public class Rulemess : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rulemess"/> class.
        /// </summary>
        public Rulemess()
        {
            Entries = new List<RulemessEntry>();
        }

        /// <summary>
        /// Gets or sets the list of <see cref="RulemessEntry" />.
        /// </summary>
        public List<RulemessEntry> Entries { get; set; }
    }
}
