using System.Collections.Generic;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a <see cref="InfoDeckDeck"/> file.
    /// </summary>
    public class InfoDeckEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x04;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoDeckEntry"/> class.
        /// </summary>
        public InfoDeckEntry()
        {
            Text = new List<string>();
        }

        /// <summary>
        /// Gets or sets the Text page.
        /// </summary>
        public List<string> Text { get; set; }
    }
}
