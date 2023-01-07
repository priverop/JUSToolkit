using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for battle/tutorials file.
    /// </summary>
    public class BattleTutorial : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BattleTutorial"/> class.
        /// </summary>
        public BattleTutorial()
        {
            Entries = new List<BattleTutorialEntry>();
        }

        /// <summary>
        /// Gets or sets the offset where the text starts.
        /// </summary>
        public int StartingOffset { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="BattleTutorialEntry" />.
        /// </summary>
        public List<BattleTutorialEntry> Entries { get; set; }
    }
}
