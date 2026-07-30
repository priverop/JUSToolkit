using System;
using System.Text;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for Piece.bin file.
    /// </summary>
    public class Piece : IFormat
    {
        /// <summary>
        /// Gets or sets the number of entries in <see cref="Entries"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="PieceEntry" />.
        /// </summary>
        public List<PieceEntry> Entries { get; set; } = [];
    }
}
