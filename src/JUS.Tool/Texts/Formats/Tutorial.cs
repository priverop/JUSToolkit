using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for tutorials files.
    /// </summary>
    public class Tutorial : IFormat
    {
        /// <summary>
        /// Gets or sets the offset where the text starts.
        /// </summary>
        public int StartingOffset { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="TutorialEntry" />.
        /// </summary>
        public List<TutorialEntry> Entries { get; set; } = [];
    }
}
