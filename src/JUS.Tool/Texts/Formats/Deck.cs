using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for deck files.
    /// </summary>
    public class Deck : IFormat
    {
        /// <summary>
        /// Gets Header size of the Deck format.
        /// </summary>
        public readonly int HeaderSize = 0x40; // 64

        /// <summary>
        /// Gets the File size of the Deck format.
        /// </summary>
        public readonly int FileSize = 0x5C; //92

        /// <summary>
        /// Gets or sets the Name of the Deck.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Header.
        /// </summary>
        public byte[] Header { get; set; }
    }
}
