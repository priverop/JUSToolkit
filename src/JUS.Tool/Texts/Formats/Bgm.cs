using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for Bgm.bin file.
    /// </summary>
    public class Bgm : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bgm"/> class.
        /// </summary>
        public Bgm()
        {
            Entries = new List<BgmEntry>();
        }

        /// <summary>
        /// Gets or sets the number of entries in <see cref="Entries"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="BgmEntry" />.
        /// </summary>
        public List<BgmEntry> Entries { get; set; }
    }
}
