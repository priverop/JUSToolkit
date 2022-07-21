using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for pname.bin file.
    /// </summary>
    public class Pname : IFormat
    {
        /// <summary>
        /// Size of a <see cref="Pname"/> entry.
        /// </summary>
        public static readonly int EntrySize = 0x04;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pname"/> class.
        /// </summary>
        public Pname()
        {
            TextEntries = new List<string>();
        }

        /// <summary>
        /// Gets or sets the number of entries in <see cref="TextEntries"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of text entries.
        /// </summary>
        public List<string> TextEntries { get; set; }
    }
}
