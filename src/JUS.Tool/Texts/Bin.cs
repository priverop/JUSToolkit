using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts
{
    /// <summary>
    /// Text Format of standard Bin Files.
    /// </summary>
    public class Bin : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bin"/> class.
        /// </summary>
        public Bin() {
            Text = new Dictionary<string, int>();
        }

        /// <summary>
        /// Gets or sets the Texts of the File.
        /// </summary>
        public Dictionary<string, int> Text { get; set; } // String - Pointer
    }
}
