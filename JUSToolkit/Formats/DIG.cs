namespace JUSToolkit.Formats
{
    using System;
    using Yarhl.FileFormat;
    using Texim.Media.Image;

    public class DIG : Format
    {
        public PixelArray Pixels { get; set; }

        public Palette Palette { get; set; }
    }
}
