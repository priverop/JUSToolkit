using Texim.Palettes;
using Texim.Pixels;
using Yarhl.FileFormat;

namespace JUSToolkit.Formats
{
     public enum ColorFormat {
        Unknown,
        // Texture NDS formats from http://nocash.emubase.de/gbatek.htm#ds3dtextureformats
        Indexed_A3I5  = 1,    // 8  bits-> 0-4: index; 5-7: alpha
        Indexed_2bpp  = 2,    // 2  bits for 4   colors
        Indexed_4bpp  = 3,    // 4  bits for 16  colors
        Indexed_8bpp  = 4,    // 8  bits for 256 colors
        Texeled_4x4   = 5,    // 32 bits-> 2 bits per texel (only in textures)
        Indexed_A5I3  = 6,    // 8  bits-> 0-2: index; 3-7: alpha
        ABGR555_16bpp = 7,    // 16 bits BGR555 color with alpha component
        // Also common formats
        Indexed_1bpp,         // 1  bit  for 2   colors
        Indexed_A4I4,         // 8  bits-> 0-3: index; 4-7: alpha
        BGRA_32bpp,           // 32 bits BGRA color
        ABGR_32bpp,           // 32 bits ABGR color
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
    }
}
