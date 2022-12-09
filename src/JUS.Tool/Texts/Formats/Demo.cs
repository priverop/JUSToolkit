using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for Demo.bin file.
    /// </summary>
    public class Demo : IFormat
    {
        /// <summary>
        /// Gets or sets the number of entries in <see cref="Entries"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="DemoEntry" />.
        /// </summary>
        public List<DemoEntry> Entries { get; set; }

        public Demo()
        {
            Entries = new List<DemoEntry>();
        }
    }
}
