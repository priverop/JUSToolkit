using JUS.Tool.Utils;
using Texim.Images;
using Texim.Images.Standard;
using Texim.TileMaps;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converter to import a PNG image into a Dig + Atm.
    /// </summary>
    public class Png2DigAtm : IConverter<Node, NodeContainerFormat>
    {
        private readonly Node originalDig;
        private readonly Node originalAtm;

        /// <summary>
        /// Gets or sets the first transparent pixel mode.
        /// </summary>
        public bool TransparentTile { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2DigAtm"/> converter.
        /// </summary>
        /// <param name="dig">Original Dig.</param>
        /// <param name="atm">Original Atm.</param>
        /// <param name="insertTransparent">The first pixel of the image is transparent.</param>
        public Png2DigAtm(Node dig, Node atm, bool insertTransparent)
        {
            originalDig = dig;
            originalAtm = atm;
            TransparentTile = insertTransparent;
        }

        /// <summary>
        /// Imports the png into Dig + Atm.
        /// </summary>
        /// <param name="png">The Node with the png to import.</param>
        /// <returns>NFC with the Dig and Atm.</returns>
        /// <exception cref="ArgumentException">If png Node is null.</exception>
        public NodeContainerFormat Convert(Node png)
        {
            ArgumentNullException.ThrowIfNull(png);

            bool digIsCompressed = CompressionUtils.IsCompressed(originalDig);
            Node uncompressedDig = digIsCompressed ?
                originalDig.TransformWith<LzssDecompression>() :
                originalDig;

            Dig dig = uncompressedDig
                .TransformWith<Binary2Dig>()
                .GetFormatAs<Dig>() ?? throw new FormatException("Invalid dig file");

            // Original Atm
            bool atmIsCompressed = CompressionUtils.IsCompressed(originalAtm);
            Node uncompressedAtm = atmIsCompressed ?
                originalAtm.TransformWith<LzssDecompression>() :
                originalAtm;

            Almt atm = uncompressedAtm
                    .TransformWith<Binary2Almt>()
                    .GetFormatAs<Almt>() ?? throw new FormatException("Invalid atm file");

            // Transform PNG into a RgbImage (Pixels + Map) using the Dig Palette
            var compressionParams = new RgbImageMapCompressionParams {
                Palettes = new Texim.Palettes.PaletteCollection(dig.Palettes.Skip(12)),
            };

            png.Stream!.Position = 0;
            MapCompressedIndexedImage compressed = png
                .TransformWith<StandardBinaryImage2RgbImage>()
                .TransformWith(new RgbImageMapCompression(compressionParams))
                .GetFormatAs<MapCompressedIndexedImage>()!;

            var newImage = new IndexedImage {
                Width = 8,
                Height = compressed.Tiles.Length / 8,
                Pixels = compressed.Tiles,
            };
            ITileMap map = compressed.Map;

            for (int i = 0; i < map.Maps.Length; i++) {
                if (map.Maps[i].PaletteIndex == 0) {
                    map.Maps[i] = map.Maps[i] with { PaletteIndex = 12 };
                }
            }

            // New Dig: original dig changing height, width and pixels
            var newDig = new Dig(dig, newImage);

            if (TransparentTile) {
                newDig = newDig.InsertTransparentTile(map);
            }

            BinaryFormat binaryDig = new Dig2Binary().Convert(newDig);

            BinaryFormat compressedDig = digIsCompressed ?
                new LzssCompression().Convert(binaryDig) :
                binaryDig;

            var transformedFiles = new NodeContainerFormat();

            transformedFiles.Root.Add(new Node(originalDig.Name, compressedDig));

            // New Atm: original atm changing height, width and maps
            var newAtm = new Almt(atm, map);
            BinaryFormat binaryAtm = new Almt2Binary().Convert(newAtm);

            BinaryFormat compressedAtm = atmIsCompressed ?
                new LzssCompression().Convert(binaryAtm) :
                binaryAtm;

            transformedFiles.Root.Add(new Node(originalAtm.Name, compressedAtm));

            return transformedFiles;
        }
    }
}
