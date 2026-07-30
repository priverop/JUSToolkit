using System;
using System.Text;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
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
        public List<DemoEntry> Entries { get; set; } = [];
    }
}
