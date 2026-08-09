using Texim.Games.Nitro.Sprites;
using Texim.Images;
using Texim.Images.Standard;
using Texim.Palettes;
using Texim.Pixels;
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converter to import PNG sprites into a DTX3 container, replacing its sprites and image.
    /// </summary>
    public class Png2Dtx3 : IConverter<NodeContainerFormat, NodeContainerFormat>
    {
        private readonly NodeContainerFormat pngs;

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2Dtx3"/> class.
        /// </summary>
        /// <param name="pngs">Container with the PNG nodes to import. Each node name must match a sprite name of the DTX3.</param>
        public Png2Dtx3(NodeContainerFormat pngs)
        {
            this.pngs = pngs;
        }

        /// <summary>
        /// Imports the PNGs into the DTX3.
        /// </summary>
        /// <param name="dtx3">The DTX3 container (sprites + image) to update.</param>
        /// <returns>NFC with the sprites and image updated.</returns>
        /// <exception cref="ArgumentException">If dtx3 is null.</exception>
        public NodeContainerFormat Convert(NodeContainerFormat dtx3)
        {
            ArgumentNullException.ThrowIfNull(dtx3);

            // Original image
            Dig originalImage = dtx3.Root.Children["image"]!.GetFormatAs<Dig>()!;
            var palettes = new PaletteCollection();
            foreach (IPalette p in originalImage.Palettes) {
                palettes.Palettes.Add(p);
            }

            // Configuration for the Converters
            var newPixels = new List<IndexedPixel>();

            var segmentation = new NitroImageSegmentation() {
                CanvasWidth = 256,
                CanvasHeight = 256,
            };
            var spriteConverterParameters = new RgbImage2SpriteParams {
                Palettes = palettes,
                IsImageTiled = true,
                MinimumPixelsPerSegment = 64,
                PixelsPerIndex = 64,
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                PixelSequences = newPixels,
                Segmentation = segmentation,
            };

            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                FullImage = originalImage,
            };

            // Iterate all the sprites to compose a new bae image.
            // If it finds the same sprites in the inserted PNGs, it will use them,
            // if not, it will use the originals.
            foreach (Node spriteNode in dtx3.Root.Children["sprites"]!.Children) {
                Node? pngNode = pngs.Root.Children[$"{spriteNode.Name}.png"];

                RgbImage image;

                if (pngNode is not null) {
                    pngNode.Stream!.Position = 0;
                    // PNG -> RgbImage
                    image = pngNode.TransformWith<StandardBinaryImage2RgbImage>().GetFormatAs<RgbImage>()!;
                } else {
                    // Sprite -> RgbImage
                    // This will re-compress the original sprites and fill up the
                    // new base image (newPixels).
                    image = new Node(spriteNode.Name, spriteNode.GetFormatAs<Sprite>()!)
                        .TransformWith(new Sprite2IndexedImage(spriteParams))
                        .TransformWith(new IndexedImage2RgbImage(palettes))
                        .GetFormatAs<RgbImage>()!;
                }

                // RgbImage -> Sprite
                ISprite sprite = new RgbImage2Sprite(spriteConverterParameters).Convert(image);

                spriteNode.ChangeFormat(sprite);
            }

            var updatedImage = new Dig(originalImage) {
                Pixels = newPixels.ToArray(),
                Width = 8,
                Height = newPixels.Count / 8,
            };

            dtx3.Root.Children["image"]!.ChangeFormat(updatedImage);

            return dtx3;
        }
    }
}
