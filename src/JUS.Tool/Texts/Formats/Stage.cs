using System;
using System.Text;
using Yarhl.FileFormat;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Format for Stage.bin file.
    /// </summary>
    public class Stage : IFormat
    {
        /// <summary>
        /// Gets or sets the list of <see cref="StageEntry" />.
        /// </summary>
        public List<StageEntry> Entries { get; set; } = [];
    }
}
