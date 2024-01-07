using System.Collections.Generic;

namespace JUSToolkit.Texts
{
    /// <summary>
    /// Class with utilities to work with Jus text files with relative pointers.
    /// </summary>
    public class DirectTextWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectTextWriter"/> class.
        /// </summary>
        public DirectTextWriter()
        {
            Strings = new List<string>();
            PointerAccumulator = 0;
        }

        /// <summary>
        /// Gets or sets the list of sentences / entries.
        /// </summary>
        public List<string> Strings { get; set; }

        /// <summary>
        /// Gets or sets the acumulator pointer.
        /// </summary>
        public int PointerAccumulator { get; set; }
    }
}
