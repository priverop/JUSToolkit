using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for chr_b_t.bin file.
    /// </summary>
    public class BtlChr : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BtlChr"/> class.
        /// </summary>
        public BtlChr()
        {
            Entries = new List<BtlChrEntry>();
        }

        /// <summary>
        /// Gets or sets the list of <see cref="BtlChrEntry" />.
        /// </summary>
        public List<BtlChrEntry> Entries { get; set; }
    }
}
