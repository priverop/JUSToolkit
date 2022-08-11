using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;

namespace JUSToolkit.Graphics
{
    /// <summary>
    /// Image format.
    /// </summary>
    public class Dig : IFormat
    {
        /// <summary>
        /// The Magic ID of the file.
        /// </summary>
        public const string STAMP = "DSIG";

        /// <summary>
        /// Initializes an instance of Dig with the PaletteCollection as well.
        /// </summary>
        public Dig() {
            PaletteCollection = new PaletteCollection();
        }

        /// <summary>
        /// Gets or sets the first byte of the format. Maybe the Type?.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the ImageFormat.
        /// </summary>
        /// <remarks>Different than 0x10, then 8 bpp.</remarks>
        public byte ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the NumPalettes.
        /// </summary>
        /// <remarks>4bpp: NumPalettes * 32 + 12 (0xC) = First Pixel.</remarks>
        public ushort NumPalettes { get; set; }

        /// <summary>
        /// Gets or sets the Width of the image.
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// Gets or sets the Height of the image.
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// Gets or sets the Palettes of the image.
        /// </summary>
        public PaletteCollection PaletteCollection { get; set; }

        /// <summary>
        /// Gets or sets the Pixels of the image.
        /// </summary>
        public IndexedPixel[] Pixels { get; set; }

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
