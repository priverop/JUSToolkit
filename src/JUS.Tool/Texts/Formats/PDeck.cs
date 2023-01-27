using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for pDeck files.
    /// </summary>
    public class PDeck : IFormat
    {
        /// <summary>
        /// Gets Header size of the PDeck format.
        /// </summary>
        public readonly int HeaderSize = 0x14; // 20 bytes

        /// <summary>
        /// Gets the File size of the PDeck format.
        /// </summary>
        public readonly int FileSize = 0x40; // 64 bytes

        /// <summary>
        /// Gets the Position of the Unkown int.
        /// </summary>
        public readonly int UnkownPosition = 0x34; // 52 bytes

        /// <summary>
        /// Gets or sets the Name of the PDeck.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Header.
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// Gets or sets the Unknown.
        /// </summary>
        public int Unknown { get; set; }
    }
}
