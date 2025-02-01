using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Base class for InfoDeck formats.
    /// </summary>
    public abstract class InfoDeckBase : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoDeckBase"/> class.
        /// </summary>
        protected InfoDeckBase()
        {
            Entries = new List<InfoDeckEntry>();
        }

        /// <summary>
        /// Gets or sets the number of entries in <see cref="Entries"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of entries (strings).
        /// </summary>
        public List<InfoDeckEntry> Entries { get; set; }

        /// <summary>
        /// Gets the number of lines per page.
        /// </summary>
        public abstract int LinesPerPage { get; }
    }
}
