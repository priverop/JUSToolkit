using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for battle/tutorials file.
    /// </summary>
    public class Tutorial : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tutorial"/> class.
        /// </summary>
        public Tutorial()
        {
            Entries = new List<TutorialEntry>();
        }

        /// <summary>
        /// Gets or sets the offset where the text starts.
        /// </summary>
        public int StartingOffset { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="TutorialEntry" />.
        /// </summary>
        public List<TutorialEntry> Entries { get; set; }
    }
}
