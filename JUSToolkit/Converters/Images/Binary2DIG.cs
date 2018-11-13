namespace JUSToolkit.Converters.Images
{
    using System;
    using Yarhl.FileFormat;
    using JUSToolkit.Formats;
    using Yarhl.IO;
    using Texim.Media.Image;
    using System.Drawing;

    public class Binary2DIG : IConverter<BinaryFormat, DIG>
    {
        public DIG Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream);

            DIG dig = new DIG();
            reader.ReadBytes(4); // Magic
            reader.ReadByte(); // Type
            reader.ReadByte(); // Type_alt
            reader.ReadByte(); // PaletteNumber
            reader.ReadByte(); // Unknown BitDepth?
            reader.ReadInt16(); // Unk
            reader.ReadInt16(); // Unk

            dig.Palette = new Palette(reader.ReadBytes(0x200).ToBgr555Colors());

            dig.Pixels = new PixelArray
            {
                Width = 256,
                Height = 192,
            };

            reader.Stream.Position = 0x20C;

            dig.Pixels.SetData(
                reader.ReadBytes((int)reader.Stream.Length - 0x20C),
                PixelEncoding.HorizontalTiles,
                ColorFormat.Indexed_8bpp,
                new Size(8, 8));
                

            return dig;
        }
    }
}
