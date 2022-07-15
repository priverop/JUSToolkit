using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{

    /// <summary>
    /// Single entry in a Bgm file.
    /// </summary>
    public class BgmEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x18;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the first description line.
        /// </summary>
        public string Desc1 { get; set; }

        /// <summary>
        /// Gets or sets the second description line.
        /// </summary>
        public string Desc2 { get; set; }

        /// <summary>
        /// Gets or sets the third description line.
        /// </summary>
        public string Desc3 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk1 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk2 { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public int Icon { get; set; }
    }
}
