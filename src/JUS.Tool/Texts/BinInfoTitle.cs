using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Formats
{
    /// <summary>
    /// Text Format BinInfoTitle (Titles of the Komas??).
    /// </summary>
    public class BinInfoTitle : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinInfoTitle"/> class.
        /// </summary>
        public BinInfoTitle()
        {
            Text = new List<string>();
            Pointers = new List<int>();
        }

        /// <summary>
        /// Gets or sets the list of Texts.
        /// </summary>
        public List<string> Text { get; set; }

        /// <summary>
        /// Gets or sets the Pointers of the File.
        /// </summary>
        public List<int> Pointers { get; set; }
    }
}
