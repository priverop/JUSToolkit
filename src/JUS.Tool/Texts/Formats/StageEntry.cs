using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a Stage file.
    /// </summary>
    public class StageEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x08;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk1 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk2 { get; set; }
    }
}
