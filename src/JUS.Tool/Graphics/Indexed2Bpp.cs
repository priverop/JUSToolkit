namespace Texim.Pixels
{
    public class Indexed2Bpp : BytePixelEncoding
    {
        public static Indexed2Bpp Instance { get; } = new Indexed2Bpp();

        public override int BitsPerPixel => 2;

        protected override IndexedPixel BitsToPixel(byte data) => new IndexedPixel(data);

        protected override byte PixelToBits(IndexedPixel pixel) => (byte)(pixel.Index & 0b0011);
    }
}
