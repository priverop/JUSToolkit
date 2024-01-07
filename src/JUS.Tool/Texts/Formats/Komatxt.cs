using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for Komatxt.bin file.
    /// </summary>
    public class Komatxt : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Komatxt"/> class.
        /// </summary>
        public Komatxt()
        {
            Entries = new List<KomatxtEntry>();
        }

        /// <summary>
        /// Gets or sets the list of <see cref="KomatxtEntry" />.
        /// </summary>
        public List<KomatxtEntry> Entries { get; set; }
    }
}
