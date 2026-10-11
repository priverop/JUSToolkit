using Texim.Formats.ImageSharp.Images;
using Texim.Images;
using Texim.Images.Quantization;
using Texim.Pixels;
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Imports a PNG (koma) into a DTX4 (NCF).
    /// </summary>
    public class Png2Dtx4 : IConverter<NodeContainerFormat, NodeContainerFormat>
    {
        private readonly IBinary png;
        private readonly KShapeSprites kshape;
        private readonly KomaElement koma;

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2Dtx4"/> class.
        /// </summary>
        /// <param name="png">The PNG to import.</param>
        /// <param name="kshape">KShape.</param>
        /// <param name="koma">Koma element.</param>
        public Png2Dtx4(IBinary png, KShapeSprites kshape, KomaElement koma)
        {
            ArgumentNullException.ThrowIfNull(png);
            ArgumentNullException.ThrowIfNull(kshape);
            ArgumentNullException.ThrowIfNull(koma);

            this.png = png;
            this.kshape = kshape;
            this.koma = koma;
        }

        /// <summary>
        /// Imports the PNG into the DTX4 (NCF).
        /// </summary>
        /// <param name="dtx4">The DTX4 (NCF) to update.</param>
        /// <returns>DTX4 (NCF) with the image updated.</returns>
        /// <exception cref="ArgumentException">If dtx4 is null.</exception>
        public NodeContainerFormat Convert(NodeContainerFormat dtx4)
        {
            ArgumentNullException.ThrowIfNull(dtx4);

            // Get the image (dig) of the dtx to get the palette
            Dig originalImage = dtx4.Root.Children["image"].GetFormatAs<Dig>();

            // Get the IndexedPixels
            png.Stream.Position = 0;
            var quantization = new FixedPaletteQuantization(originalImage.Palettes[0]);
            RgbImage rgbImage = new StandardBinaryImage2RgbImage().Convert(png);
            IndexedPaletteImage newImage = new RgbImage2IndexedPaletteImage(quantization).Convert(rgbImage);

            // We ignore the sprite info from the DSTX and we take the one from the kshape
            Sprite sprite = kshape.GetSprite(koma.KShapeGroupId, koma.KShapeElementId);

            // We need to create a new IndexedImage getting the segment pixels and adding them at the end of the image.
            // Sorted image (the one we import) -> Unsorted image (the DTX store them like that)
            var segmentedImage = new List<IndexedPixel>();
            foreach (IImageSegment segment in sprite.Segments) {
                IndexedImage segmentImage = newImage.SubImage(segment.ToRectangle());
                segmentedImage.AddRange(segmentImage.Pixels);
            }

            // We do swizzling now, instead of waiting the Dig2Binary converter to do it because the game
            // does swizzling with width 48 (segments width), but the converter will do it with the image width 8.
            // Providing the swizzled pixels now, and letting Dig2Binary do swizzling again it's not an issue because
            // swizzling with width 8 (and tile size 8x8) does nothing. So when the Dig2Binary converter swizzles a second time
            // with the image width of 8, it will get the same pixels as give it now swizzled at 48.
            IndexedPixel[] tiledPixels = new TileSwizzling<IndexedPixel>(48).Swizzle(segmentedImage);

            // Update image with the new changes
            Dig updatedImage = new Dig(originalImage) {
                Pixels = tiledPixels,
                Width = 8,
                Height = tiledPixels.Length / 8,
            }.InsertTransparentTile();

            dtx4.Root.Children["image"].ChangeFormat(updatedImage);

            return dtx4;
        }
    }
}
