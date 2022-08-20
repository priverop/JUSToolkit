using System.Linq;
using Texim.Images;
using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;

namespace JUSToolkit.Graphics
{
    /// <summary>
    /// Bpp of a <see cref="Dig"/> image.
    /// </summary>
    public enum DigBpp
    {
        /// <summary>
        /// 4 bpp mode.
        /// </summary>
        Bpp4 = 0,

        /// <summary>
        /// 8 bpp mode.
        /// </summary>
        Bpp8 = 1,
    }

    /// <summary>
    /// Swizzling of a <see cref="Dig"/> image.
    /// </summary>
    public enum DigSwizzling
    {
        /// <summary>
        /// Tiled swizzling
        /// </summary>
        Tiled = 1,

        /// <summary>
        /// Linear swizzling
        /// </summary>
        Linear = 2,
    }

    /// <summary>
    /// Image format.
    /// </summary>
    public class Dig : IndexedPaletteImage, IFormat
    {
        /// <summary>
        /// The Magic ID of the file.
        /// </summary>
        public const string STAMP = "DSIG";

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class.
        /// </summary>
        public Dig() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class cloning another Dig object.
        /// </summary>
        /// <param name="dig">Dig object to clone.</param>
        public Dig(Dig dig) {
            Unknown = dig.Unknown;
            ImageFormat = dig.ImageFormat;
            NumPaletteLines = dig.NumPaletteLines;
            Width = dig.Width;
            Height = dig.Height;
            Pixels = dig.Pixels;
            PaletteStart = dig.PaletteStart;
            PixelsStart = dig.PixelsStart;
            Bpp = dig.Bpp;
            Swizzling = dig.Swizzling;
            foreach (IPalette p in dig.Palettes) {
                Palettes.Add(p);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dig"/> class cloning the indexed image.
        /// </summary>
        /// <param name="dig">Dig object to clone.</param>
        /// <param name="image">IndexedImage object to clone.</param>
        public Dig(Dig dig, IIndexedImage image)
            : this(dig)
        {
            Height = image.Height;
            Width = image.Width;
            Pixels = image.Pixels.ToArray();
        }

        /// <summary>
        /// Gets or sets the first byte of the format. Maybe the Type?.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the ImageFormat.
        /// </summary>
        public byte ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the NumPaletteLines.
        /// </summary>
        public ushort NumPaletteLines { get; set; }

        /// <summary>
        /// Gets or sets the PaletteStart value.
        /// </summary>
        public uint PaletteStart { get; set; }

        /// <summary>
        /// Gets or sets the PixelsStart value.
        /// </summary>
        public uint PixelsStart { get; set; }

        /// <summary>
        /// Gets or sets the Bpp mode.
        /// </summary>
        public DigBpp Bpp { get; set; }

        /// <summary>
        /// Gets or sets the Swizzling mode.
        /// </summary>
        public DigSwizzling Swizzling { get; set; }
    }
}
