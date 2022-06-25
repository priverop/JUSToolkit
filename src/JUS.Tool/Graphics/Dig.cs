using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;

namespace JUSToolkit.Graphics
{
    /// <summary>
    /// Texture NDS formats from http://nocash.emubase.de/gbatek.htm#ds3dtextureformats.
    /// </summary>
    public enum ColorFormat
    {
        /// <summary>
        /// Unknown Color Format.
        /// </summary>
        Unknown,

        /// <summary>
        ///  8  bits-> 0-4: index; 5-7: alpha
        /// </summary>
        Indexed_A3I5 = 1,    // 8  bits-> 0-4: index; 5-7: alpha

        /// <summary>
        ///  2  bits for 4   colors
        /// </summary>
        Indexed_2bpp = 2,    // 2  bits for 4   colors

        /// <summary>
        ///  4  bits for 16  colors
        /// </summary>
        Indexed_4bpp = 3,

        /// <summary>
        ///  8  bits for 256 colors
        /// </summary>
        Indexed_8bpp = 4,

        /// <summary>
        ///  32 bits-> 2 bits per texel (only in textures)
        /// </summary>
        Texeled_4x4 = 5,

        /// <summary>
        ///  8  bits-> 0-2: index; 3-7: alpha
        /// </summary>
        Indexed_A5I3 = 6,

        /// <summary>
        ///  16 bits BGR555 color with alpha component
        /// </summary>
        ABGR555_16bpp = 7,

        /// <summary>
        ///  1  bit  for 2 colors
        /// </summary>
        Indexed_1bpp,

        /// <summary>
        ///  8  bits-> 0-3: index; 4-7: alpha
        /// </summary>
        Indexed_A4I4,

        /// <summary>
        ///  32 bits BGRA color
        /// </summary>
        BGRA_32bpp,

        /// <summary>
        ///  32 bits ABGR color
        /// </summary>
        ABGR_32bpp,
    }

    /// <summary>
    /// Image format.
    /// </summary>
    public class Dig : IFormat
    {
        /// <summary>
        /// Gets or sets the Pixels of the image.
        /// </summary>
        public IndexedPixel[] Pixels { get; set; }

        /// <summary>
        /// Gets or sets the Palette of the image.
        /// </summary>
        public PaletteCollection Palettes { get; set; }

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

        /// <summary>
        /// Gets or sets the ColorFormat value.
        /// </summary>
        public ColorFormat ColorFormat { get; set; }
    }
}
