using System.Collections.Generic;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a <see cref="Piece"/> file.
    /// </summary>
    public class PieceEntry : IEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x60;

        /// <summary>
        /// Number of authors / info entries.
        /// </summary>
        public static readonly int NumInfo = 2;

        /// <summary>
        /// Maximum line length for info/author sections.
        /// </summary>
        public static readonly int MaxLineLengthInfo = 19;

        /// <summary>
        /// Lines per description page.
        /// </summary>
        public static readonly int LinesPerPage = 9;

        /// <summary>
        /// Lines per info section.
        /// </summary>
        public static readonly int LinesPerInfo = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="PieceEntry"/> class.
        /// </summary>
        public PieceEntry()
        {
            Authors = new List<string>();
            Info = new List<string>();
            Page1 = new List<string>();
            Page2 = new List<string>();
        }

        /// <inheritdoc/>
        public int MaxLineLength => 40;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        public List<string> Authors { get; set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        public List<string> Info { get; set; }

        /// <summary>
        /// Gets or sets first page.
        /// </summary>
        public List<string> Page1 { get; set; }

        /// <summary>
        /// Gets or sets second page.
        /// </summary>
        public List<string> Page2 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk1 { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public short Id { get; set; }
    }
}
