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
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using NUnit.Framework;
using Texim.Formats;
using Texim.Images;
using Texim.Pixels;
using Texim.Processing;
using Texim.Sprites;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Graphics
{
    [TestFixture]
    // ToDo: DTX3 sprite deserialize & Hash, and DTX3 sprite export and import
    public class DtxTests
    {
        public static IEnumerable<TestCaseData> GetKomaFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "komas.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]),
                    Path.Combine(basePath, data[2]),
                    Path.Combine(basePath, data[3]))
                    .SetName($"({data[0]}, {data[1]}, {data[2]}, {data[3]})"));
        }

        public static IEnumerable<TestCaseData> GetDtx4Files()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "dtx4.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]),
                    Path.Combine(basePath, data[2]),
                    Path.Combine(basePath, data[3]),
                    Path.Combine(basePath, data[4]))
                    .SetName($"({data[0]}, {data[1]}, {data[2]}, {data[3]}, {data[4]})"));
        }

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

        [TestCaseSource(nameof(GetKomaFiles))]
        public void DeserializeKomas(string infoPath, string container, string koma, string kshape)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(container);
            TestDataBase.IgnoreIfFileDoesNotExist(koma);
            TestDataBase.IgnoreIfFileDoesNotExist(kshape);

            var expected = NodeContainerInfo.FromYaml(infoPath);
            var resultContainer = new NodeContainerFormat();

            Node images = NodeFactory.FromFile(container)
                .TransformWith<Binary2Alar3>()
                .Children["koma"] ?? throw new FormatException("Invalid container file");

            KShapeSprites shapes = NodeFactory.FromFile(kshape)
                .TransformWith<BinaryKShape2SpriteCollection>()
                .GetFormatAs<KShapeSprites>();

            Koma komaFormat = NodeFactory.FromFile(koma)
                .TransformWith<Binary2Koma>()
                .GetFormatAs<Koma>();

            foreach (KomaElement komaElement in komaFormat) {
                string filename = $"{komaElement.KomaName}.dtx";

                Node dtx = images.Children[filename];
                if (dtx is null) {
                    continue;
                }

                dtx.TransformWith<BinaryDtx4ToSpriteImage>();
                IndexedPaletteImage image = dtx.Children["image"].GetFormatAs<IndexedPaletteImage>();

                // We ignore the sprite info from the DSTX and we take the one
                // from the kshape
                Sprite sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

                // If the child Node komaElement.KShapeGroupId does not exist, then we create it
                if (resultContainer.Root.Children[$"{komaElement.KShapeGroupId}"] == null) {
                    resultContainer.Root.Add(new Node($"{komaElement.KShapeGroupId}"));
                }

                var spriteParams = new Sprite2IndexedImageParams {
                    RelativeCoordinates = SpriteRelativeCoordinatesKind.TopLeft,
                    FullImage = image,
                };
                var indexedImageParams = new IndexedImageBitmapParams {
                    Palettes = image,
                };
                Node resultImage = new Node(komaElement.KomaName, sprite)
                    .TransformWith(new Sprite2IndexedImage(spriteParams))
                    .TransformWith(new IndexedImage2Bitmap(indexedImageParams));
                resultContainer.Root.Children[$"{komaElement.KShapeGroupId}"].Add(resultImage);
            }

            resultContainer.Root.Should().MatchInfo(expected);
        }

        [TestCaseSource(nameof(GetDtx4Files))]
        public void DeserializeDtx4AndCheckFileHash(string infoPath, string dtxPath, string container, string koma, string kshape)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(dtxPath);
            TestDataBase.IgnoreIfFileDoesNotExist(container);
            TestDataBase.IgnoreIfFileDoesNotExist(koma);
            TestDataBase.IgnoreIfFileDoesNotExist(kshape);

            var info = BinaryInfo.FromYaml(infoPath);

            // Sprites + pixels + palette
            using Node dtx4 = NodeFactory.FromFile(dtxPath, FileOpenMode.Read)
            .TransformWith<LzssDecompression>()
            .TransformWith<BinaryDtx4ToSpriteImage>(); // NCF with sprite+image

            IndexedPaletteImage image = dtx4.Children["image"].GetFormatAs<IndexedPaletteImage>();

            KShapeSprites shapes = NodeFactory.FromFile(kshape)
                .TransformWith<BinaryKShape2SpriteCollection>()
                .GetFormatAs<KShapeSprites>();

            Koma komaFormat = NodeFactory.FromFile(koma)
                .TransformWith<Binary2Koma>()
                .GetFormatAs<Koma>();

            KomaElement komaElement = komaFormat.First(n => n.KomaName == Path.GetFileNameWithoutExtension(dtxPath)) ?? throw new FormatException("Can't find the dtx in the koma.bin");

            Sprite sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.TopLeft,
                FullImage = image,
            };
            var indexedImageParams = new IndexedImageBitmapParams {
                Palettes = image,
            };

            new Node("sprite", sprite)
                .TransformWith(new Sprite2IndexedImage(spriteParams))
                .TransformWith(new IndexedImage2Bitmap(indexedImageParams))
                .Stream.Should().MatchInfo(info);
        }

        [TestCaseSource(nameof(GetDtx4Files))]
        public void TwoWaysIdenticalDtx4(string infoPath, string dtxPath, string container, string koma, string kshape)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(dtxPath);
            TestDataBase.IgnoreIfFileDoesNotExist(container);
            TestDataBase.IgnoreIfFileDoesNotExist(koma);
            TestDataBase.IgnoreIfFileDoesNotExist(kshape);

            // Sprites + pixels + palette
            using Node dtx4 = NodeFactory.FromFile(dtxPath, FileOpenMode.Read)
            .TransformWith<LzssDecompression>();
            var originalDtx = (BinaryFormat)new BinaryFormat(dtx4.Stream).DeepClone();

            dtx4.TransformWith<BinaryDtx4ToSpriteImage>(); // NCF with sprite+image

            Dig originalImage = dtx4.Children["image"].GetFormatAs<Dig>();

            KShapeSprites shapes = NodeFactory.FromFile(kshape)
                .TransformWith<BinaryKShape2SpriteCollection>()
                .GetFormatAs<KShapeSprites>();

            Koma komaFormat = NodeFactory.FromFile(koma)
                .TransformWith<Binary2Koma>()
                .GetFormatAs<Koma>();

            KomaElement komaElement = komaFormat.First(n => n.KomaName == Path.GetFileNameWithoutExtension(dtxPath)) ?? throw new FormatException("Can't find the dtx in the koma.bin");

            Sprite sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

            var spriteParams = new Sprite2IndexedImageParams {
                RelativeCoordinates = SpriteRelativeCoordinatesKind.TopLeft,
                FullImage = originalImage,
            };
            var indexedImageParams = new IndexedImageBitmapParams {
                Palettes = originalImage,
            };

            var pngNode = new Node("sprite", sprite)
                .TransformWith(new Sprite2IndexedImage(spriteParams))
                .TransformWith(new IndexedImage2Bitmap(indexedImageParams));

            pngNode.Stream.Position = 0;

            // Import
            var quantization = new FixedPaletteQuantization(originalImage.Palettes[0]);
            pngNode.TransformWith<Bitmap2FullImage>()
                .TransformWith(new FullImage2IndexedPalette(quantization));
            IndexedPaletteImage newImage = pngNode.GetFormatAs<IndexedPaletteImage>();

            var segmentedImage = new List<IndexedPixel>();
            foreach (IImageSegment segment in sprite.Segments) {
                IndexedImage segmentImage = newImage.SubImage(segment.CoordinateX, segment.CoordinateY, segment.Width, segment.Height);
                segmentedImage.AddRange(segmentImage.Pixels);
            }

            // Linear image to Tiled image (how the DTX stores them)
            IndexedPixel[] tiledPixels = new TileSwizzling<IndexedPixel>(48).Swizzle(segmentedImage);

            // Update image with the new changes
            Dig updatedImage = new Dig(originalImage) {
                Pixels = tiledPixels.ToArray(),
                Width = 8,
                Height = tiledPixels.Length / 8,
                Swizzling = DigSwizzling.Linear,
            }.InsertTransparentTile();

            dtx4.Children["image"].ChangeFormat(updatedImage);

            DataStream generatedStream = new Dtx4ToBinary().Convert(dtx4.GetFormatAs<NodeContainerFormat>())
                .Stream;

            // Compare
            var originalStream = new DataStream(originalDtx.Stream!, 0, originalDtx.Stream.Length);
            generatedStream.Length.Should().Be(originalStream.Length);
            originalStream.WriteTo("original.dtx");
            generatedStream.WriteTo("generated.dtx");
            generatedStream.Compare(originalStream).Should().BeTrue();
        }

        [TestCaseSource(nameof(GetDtx3TxFiles))]
        public void DeserializeDtx3TxAndCheckFileHash(string infoPath, string dtxPath)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(dtxPath);

            var info = BinaryInfo.FromYaml(infoPath);

            using Node dtx = NodeFactory.FromFile(dtxPath, FileOpenMode.Read)
                .TransformWith(new BinaryToDtx3());

            Dig image = dtx.Children["image"].GetFormatAs<Dig>();
            var indexedImageParams = new IndexedImageBitmapParams {
                Palettes = image,
            };

            BinaryFormat generatedStream = new IndexedImage2Bitmap(indexedImageParams).Convert(image);

            generatedStream.Should().MatchInfo(info);
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
