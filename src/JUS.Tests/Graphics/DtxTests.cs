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
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using NUnit.Framework;
using Texim.Formats;
using Texim.Images;
using Texim.Sprites;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Tests.Containers
{
    [TestFixture]
    public class DtxTests
    {
        public static IEnumerable<TestCaseData> GetDtxFiles()
        {
            string basePath = Path.Combine(TestDataBase.RootFromOutputPath, "Graphics");
            string listPath = Path.Combine(basePath, "dtx.txt");
            return TestDataBase.ReadTestListFile(listPath)
                .Select(line => line.Split(','))
                .Select(data => new TestCaseData(
                    Path.Combine(basePath, data[0]),
                    Path.Combine(basePath, data[1]),
                    Path.Combine(basePath, data[2]),
                    Path.Combine(basePath, data[3]))
                    .SetName($"({data[0]}, {data[1]}, {data[2]}, {data[3]})"));
        }

        [TestCaseSource(nameof(GetDtxFiles))]
        public void DeserializeDtx(string infoPath, string container, string koma, string kshape)
        {
            TestDataBase.IgnoreIfFileDoesNotExist(infoPath);
            TestDataBase.IgnoreIfFileDoesNotExist(container);
            TestDataBase.IgnoreIfFileDoesNotExist(koma);
            TestDataBase.IgnoreIfFileDoesNotExist(kshape);

            var expected = NodeContainerInfo.FromYaml(infoPath);
            var resultContainer = new NodeContainerFormat();

            Node images = NodeFactory.FromFile(container)
                .TransformWith<Binary2Alar3>()
                .Children["koma"];
            if (images is null) {
                throw new FormatException("Invalid container file");
            }

            var shapes = NodeFactory.FromFile(kshape)
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

                dtx.TransformWith<BinaryDstx2SpriteImage>();
                var image = dtx.Children["image"].GetFormatAs<IndexedPaletteImage>();

                // We ignore the sprite info from the DSTX and we take the one
                // from the kshape
                var sprite = shapes.GetSprite(komaElement.KShapeGroupId, komaElement.KShapeElementId);

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
                var resultImage = new Node(komaElement.KomaName, sprite)
                    .TransformWith<Sprite2IndexedImage, Sprite2IndexedImageParams>(spriteParams)
                    .TransformWith<IndexedImage2Bitmap, IndexedImageBitmapParams>(indexedImageParams);
                resultContainer.Root.Children[$"{komaElement.KShapeGroupId}"].Add(resultImage);
            }

            resultContainer.Root.Should().MatchInfo(expected);
        }
    }
}
