using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a <see cref="Piece"/> file.
    /// </summary>
    public class PieceEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x60;

        /// <summary>
        /// Number of authors.
        /// </summary>
        public static readonly int NumAuthors = 2;

        /// <summary>
        /// Number of info.
        /// </summary>
        public static readonly int NumInfo = 2;

        /// <summary>
        /// Lines per page.
        /// </summary>
        public static readonly int LinesPerPage = 9;

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

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

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
