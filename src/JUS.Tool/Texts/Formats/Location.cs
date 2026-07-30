using System;
using System.Text;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for Location.bin file.
    /// </summary>
    public class Location : IFormat
    {
        /// <summary>
        /// Gets or sets the number of entries in <see cref="Entries"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="LocationEntry" />.
        /// </summary>
        public List<LocationEntry> Entries { get; set; } = [];
    }
}
