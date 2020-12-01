namespace JUSToolkit.Formats
{
    using System;
    using Yarhl.FileFormat;
    using Texim;

    public class DIG : Format
    {
        public PixelArray Pixels { get; set; }

        public Palette Palette { get; set; }

        public byte[] Magic { get; set; }
        public byte Type { get; set; }
        public byte PaletteType { get; set; }
        public byte PaletteSize { get; set; } // PaletteSize * 32 + 12 = First Pixel
        public byte Unknown { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint PaletteStart { get; set; }
        public uint PixelsStart { get; set; }
    }
}
