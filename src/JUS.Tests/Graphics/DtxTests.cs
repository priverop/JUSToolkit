// Copyright(c) 2022 Priverop
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using JUS.Tool.Graphics.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using NUnit.Framework;
using Texim.Formats;
using Texim.Palettes;
using Texim.Pixels;
using Texim.Sprites;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Graphics
{
    [TestFixture]
    public class DtxTests
    {
        public static IEnumerable<TestCaseData> GetDtx3TxFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "dtx3tx.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        public static IEnumerable<TestCaseData> GetDtx3Files()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "dtx3.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        public static IEnumerable<TestCaseData> GetDtx2_3Files()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "dtx2_3.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]))
                    .SetName($"({data[0]}, {data[1]})"));
        }

        ///////////
        // TESTS //
        ///////////
        [TestCaseSource(nameof(GetDtx3Files))]
        [TestCaseSource(nameof(GetDtx2_3Files))]
        [TestCaseSource(nameof(GetDtx3TxFiles))]
        public void DeserializeDtx3AndCheckFileHash(string infoPath, string dtxPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(dtxPath);

            var info = NodeContainerInfo.FromYaml(infoPath);

            using Node dtx = NodeFactory.FromFile(dtxPath, FileOpenMode.Read)
                .TransformWith<LzssDecompression>()
                .TransformWith<Dtx2Bitmaps>();

            dtx.Should().MatchInfo(info);
        }

        [TestCaseSource(nameof(GetDtx3Files))]
        public void TwoWaysIdenticalDtx3(string infoPath, string dtxPath)
        {
            // 0 - Checks
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(dtxPath);

            // 1 - Dtx -> Pngs
            using Node dtx = NodeFactory.FromFile(dtxPath, FileOpenMode.Read)
                .TransformWith<LzssDecompression>();
            var originalDtx = (BinaryFormat)new BinaryFormat(dtx.Stream).DeepClone();

            dtx.TransformWith(new BinaryToDtx3());

            // Original image
            Dig originalImage = dtx.Children["image"].GetFormatAs<Dig>();
            var palettes = new PaletteCollection();
            foreach (IPalette p in originalImage.Palettes) {
                palettes.Palettes.Add(p);
            }

            // Configuration for the Converters
            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                FullImage = originalImage,
            };
            var indexedImageParams = new IndexedImageBitmapParams {
                Palettes = originalImage,
            };

            var newPixels = new List<IndexedPixel>();

            var segmentation = new NitroImageSegmentation() {
                CanvasWidth = 256,
                CanvasHeight = 256,
            };
            var spriteConverterParameters = new FullImage2SpriteParams {
                Palettes = palettes,
                IsImageTiled = true,
                MinimumPixelsPerSegment = 64,
                PixelsPerIndex = 64,
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                PixelSequences = newPixels,
                Segmentation = segmentation,
            };

            var originalBitmaps = new NodeContainerFormat();

            // Cloning the PNG to compare them later, as our compression algorithm is better
            // than the game's, and the new .dtx is smaller than the original
            foreach (Node spriteNode in dtx.Children["sprites"].Children) {
                // Cloning the node so we can transform it
                originalBitmaps.Root.Add(new Node(spriteNode.Name, spriteNode.GetFormatAs<Sprite>())
                            .TransformWith(new Sprite2IndexedImage(spriteParams))
                            .TransformWith(new IndexedImage2Bitmap(indexedImageParams)));
            }

            using var cloneBitmaps = (NodeContainerFormat)originalBitmaps.DeepClone();

            // 2 - Import the PNGs into the DTX
            foreach (Node pngNode in cloneBitmaps.Root.Children) {
                pngNode.Stream.Position = 0;
                pngNode.TransformWith<Bitmap2FullImage>();

                // FullImage -> Sprite
                var converter = new FullImage2Sprite(spriteConverterParameters);
                pngNode.TransformWith(converter);
                Sprite sprite = pngNode.GetFormatAs<Sprite>();

                // Check if there is a Children with the correct name:
                string cleanSpriteName = Path.GetFileNameWithoutExtension(pngNode.Name);
                Node spriteToReplace = dtx.Children["sprites"].Children[cleanSpriteName]
                ?? throw new ArgumentException($"Wrong sprite name: {cleanSpriteName}");

                spriteToReplace.ChangeFormat(sprite);
            }

            var updatedImage = new Dig(originalImage) {
                Pixels = newPixels.ToArray(),
                Width = 8,
                Height = newPixels.Count / 8,
            };

            dtx.Children["image"].ChangeFormat(updatedImage);

            BinaryFormat generatedBinary = new Dtx3ToBinary().Convert(dtx.GetFormatAs<NodeContainerFormat>());

            var originalStream = new DataStream(originalDtx.Stream!, 0, originalDtx.Stream.Length);
            originalStream.Length.Should().BeGreaterThan(generatedBinary.Stream.Length);

            NodeContainerFormat newDtx = new BinaryToDtx3().Convert(generatedBinary);

            // 3 - Compare the original PNGs and the new PNGs
            var spriteParams2 = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.Center,
                FullImage = updatedImage,
            };
            var indexedImageParams2 = new IndexedImageBitmapParams {
                Palettes = updatedImage,
            };
            for (int i = 0; i < newDtx.Root.Children["sprites"].Children.Count; i++) {
                var spriteNode = newDtx.Root.Children["sprites"].Children[i];

                // Cloning the node so we can transform it
                var pngNode = new Node(spriteNode.Name, spriteNode.GetFormatAs<Sprite>())
                            .TransformWith(new Sprite2IndexedImage(spriteParams2))
                            .TransformWith(new IndexedImage2Bitmap(indexedImageParams2));

                pngNode.Stream.Compare(originalBitmaps.Root.Children[i].Stream).Should().BeTrue();
            }
        }

        [TestCaseSource(nameof(GetDtx3TxFiles))]
        public void TwoWaysIdenticalDtx3TxYaml(string infoPath, string dtxPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(dtxPath);

            using Node dtx = NodeFactory.FromFile(dtxPath, FileOpenMode.Read);
            var originalDtx = (BinaryFormat)new BinaryFormat(dtx.Stream).DeepClone();

            dtx.TransformWith(new BinaryToDtx3());

            BinaryFormat yaml = dtx.Children["yaml"].GetFormatAs<BinaryFormat>();

            var reader = new TextDataReader(yaml.Stream);
            reader.Stream.Position = 0;

            // Import with Yaml
            var yamlConverter = new Dtx3TxToBinary(originalDtx, GetYamlInfo(reader.ReadToEnd()));

            BinaryFormat generatedStream = yamlConverter.Convert(dtx.GetFormatAs<NodeContainerFormat>());

            var originalStream = new DataStream(originalDtx.Stream!, 0, originalDtx.Stream.Length);
            generatedStream.Stream.Length.Should().Be(originalStream.Length);
            generatedStream.Stream.Compare(originalStream).Should().BeTrue();
        }

        [TestCaseSource(nameof(GetDtx3TxFiles))]
        public void TwoWaysIdenticalDtx3TxNoYaml(string infoPath, string dtxPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(dtxPath);

            using Node dtx = NodeFactory.FromFile(dtxPath, FileOpenMode.Read);
            var originalDtx = (BinaryFormat)new BinaryFormat(dtx.Stream).DeepClone();

            dtx.TransformWith(new BinaryToDtx3());

            var yamlConverter = new Dtx3TxToBinary(originalDtx);

            BinaryFormat generatedStream = yamlConverter.Convert(dtx.GetFormatAs<NodeContainerFormat>());

            var originalStream = new DataStream(originalDtx.Stream!, 0, originalDtx.Stream.Length);
            generatedStream.Stream.Length.Should().Be(originalStream.Length);
            generatedStream.Stream.Compare(originalStream).Should().BeTrue();
        }

        private static List<SpriteDummy> GetYamlInfo(string yamlText)
        {
            return new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Deserialize<List<SpriteDummy>>(yamlText);
        }
    }
}
