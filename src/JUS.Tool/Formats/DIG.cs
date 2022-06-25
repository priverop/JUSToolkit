using System;
using Texim;
using Yarhl.FileFormat;

namespace JUSToolkit.Formats
{
    /// <summary>
    /// Image format.
    /// </summary>
    public class Dig : IFormat
    {
        /// <summary>
        /// Gets or sets the Pixels of the image.
        /// </summary>
        public PixelArray Pixels { get; set; }

        /// <summary>
        /// Gets or sets the Palette of the image.
        /// </summary>
        public Palette Palette { get; set; }

        /// <summary>
        /// Gets or sets the Magic.
        /// </summary>
        public byte[] Magic { get; set; }

        /// <summary>
        /// Gets or sets the Dig Type.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets the PaletteType.
        /// </summary>
        public byte PaletteType { get; set; }

        /// <summary>
        /// Gets or sets the PaletteSize (PaletteSize * 32 + 12 = First Pixel).
        /// </summary>
        public byte PaletteSize { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the Width of the image.
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// Gets or sets the Height of the image.
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// Gets or sets the PaletteStart value.
        /// </summary>
        public uint PaletteStart { get; set; }

        /// <summary>
        /// Gets or sets the PixelsStart value.
        /// </summary>
        public uint PixelsStart { get; set; }
    }
}
