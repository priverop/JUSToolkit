using JUS.Tool.Utils;
using Texim.Formats.ImageSharp.Images;
using Texim.Images;
using Texim.Palettes;
using Texim.TileMaps;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Graphics.Converters
{
    /// <summary>
    /// Converter to import a PNG image into a Dig + Atm.
    /// </summary>
    public class Png2DigAtm :
        IConverter<Node, NodeContainerFormat>,
        IConverter<Node[], NodeContainerFormat>
    {
        private readonly Node originalDig;
        private readonly Node[] originalAtms;

        /// <summary>
        /// Gets or sets the first transparent pixel mode.
        /// </summary>
        public bool TransparentTile { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2DigAtm"/> converter for a single PNG.
        /// </summary>
        /// <param name="dig">Original Dig.</param>
        /// <param name="atm">Original Atm (tilemap).</param>
        /// <param name="insertTransparent">The first pixel of the image is transparent.</param>
        public Png2DigAtm(Node dig, Node atm, bool insertTransparent)
        {
            originalDig = dig;
            originalAtms = [atm];
            TransparentTile = insertTransparent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2DigAtm"/> converter for multiple
        /// PNGs sharing the same Dig.
        /// </summary>
        /// <param name="dig">Original Dig, shared by all the atms.</param>
        /// <param name="atms">Original Atms (tilemaps).</param>
        /// <param name="insertTransparent">The first pixel of the image is transparent.</param>
        public Png2DigAtm(Node dig, Node[] atms, bool insertTransparent)
        {
            originalDig = dig;
            originalAtms = atms;
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

            return Convert([png]);
        }

        /// <summary>
        /// Imports all the pngs into a single Dig + multiple Atms.
        /// </summary>
        /// <param name="pngs">The Nodes with the pngs to import.</param>
        /// <returns>NFC with the Dig and the Atms.</returns>
        /// <exception cref="ArgumentException">If pngs is null.</exception>
        public NodeContainerFormat Convert(Node[] pngs)
        {
            ArgumentNullException.ThrowIfNull(pngs);

            if (pngs.Length != originalAtms.Length) {
                throw new FormatException("Number of pngs and atms is different.");
            }

            var decompression = new LzssDecompression();

            // Dig
            bool digIsCompressed = CompressionUtils.IsCompressed(originalDig);
            BinaryFormat uncompressedDig = decompression.Convert(originalDig.GetFormatAs<IBinary>());
            Dig dig = new Binary2Dig().Convert(uncompressedDig) ?? throw new FormatException("Invalid dig file");

            // Convert PNG into a RgbImage (Pixels + Map) using the Dig Palette
            int paletteIndexStart = FirstNonBlackPaletteIndex(dig);
            var compressionParams = new RgbImageMapCompressionParams {
                Palettes = dig,
                PaletteIndexStart = paletteIndexStart,
            };

            var transformedFiles = new NodeContainerFormat();

            for (int i = 0; i < pngs.Length; i++) {
                pngs[i].Stream.Position = 0;
                RgbImage rgbImage = new StandardBinaryImage2RgbImage().Convert(pngs[i].GetFormatAs<IBinary>());
                MapCompressedIndexedImage compressed = new RgbImageMapCompression(compressionParams).Convert(rgbImage);

                var newImage = new IndexedImage {
                    Width = 8,
                    Height = compressed.Tiles.Length / 8,
                    Pixels = compressed.Tiles,
                };
                ITileMap map = compressed.Map;

                // New Dig: original dig changing height, width and pixels
                dig = new Dig(dig, newImage);

                if (TransparentTile && i == 0) {
                    dig = dig.InsertTransparentTile(map);
                }

                compressionParams = new RgbImageMapCompressionParams {
                    MergeImage = dig,
                    Palettes = dig,
                    PaletteIndexStart = paletteIndexStart,
                };

                // Atm
                bool atmIsCompressed = CompressionUtils.IsCompressed(originalAtms[i]);
                BinaryFormat uncompressedAtm = decompression.Convert(originalAtms[i].GetFormatAs<IBinary>());
                Altm atm = new Binary2Altm().Convert(uncompressedAtm) ?? throw new FormatException("Invalid atm file");

                // New Atm: original atm changing height, width and maps
                var newAtm = new Altm(atm, map);
                BinaryFormat binaryAtm = new Altm2Binary().Convert(newAtm);

                BinaryFormat compressedAtm = atmIsCompressed ?
                    new LzssCompression().Convert(binaryAtm) :
                    binaryAtm;

                transformedFiles.Root.Add(new Node(originalAtms[i].Name, compressedAtm));
            }

            dig.CheckMaxTiles(originalDig.Name);

            BinaryFormat binaryDig = new Dig2Binary().Convert(dig);

            BinaryFormat compressedDig = digIsCompressed ?
                new LzssCompression().Convert(binaryDig) :
                binaryDig;

            transformedFiles.Root.Add(new Node(originalDig.Name, compressedDig));

            return transformedFiles;
        }

        /// <summary>
        /// Gets the index of the first palette that is not entirely black.
        /// </summary>
        /// <param name="palettes">The palettes of the image.</param>
        /// <returns>The index of the first non-black palette, 0 if every palette is black.</returns>
        private static int FirstNonBlackPaletteIndex(IPaletteCollection palettes)
        {
            for (int i = 0; i < palettes.Palettes.Count; i++) {
                bool isBlack = palettes.Palettes[i].Colors
                    .All(color => color.Red == 0 && color.Green == 0 && color.Blue == 0);

                if (!isBlack) {
                    return i;
                }
            }

            return 0;
        }
    }
}
