using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for chr_s_t.bin file.
    /// </summary>
    public class SuppChr : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuppChr"/> class.
        /// </summary>
        public SuppChr()
        {
            Entries = new List<SuppChrEntry>();
        }

        /// <summary>
        /// Gets or sets the list of <see cref="SuppChrEntry" />.
        /// </summary>
        public List<SuppChrEntry> Entries { get; set; }
    }
}
