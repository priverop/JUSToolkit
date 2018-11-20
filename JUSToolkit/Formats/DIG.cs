namespace JUSToolkit.Formats
{
    using System;
    using Yarhl.FileFormat;
    using Texim.Media.Image;

    public class DIG : Format
    {
        public PixelArray Pixels { get; set; }

        public Palette Palette { get; set; }

        public byte[] Magic { get; set; }
        public byte Type { get; set; }
        public byte Type_alt { get; set; }
        public byte PaletteNumber { get; set; }
        public byte Unknown { get; set; }
        public ushort Unk { get; set; }
        public ushort Unk2 { get; set; }
    }
}
