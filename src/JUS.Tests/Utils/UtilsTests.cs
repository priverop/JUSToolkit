// Copyright (c) 2020 Priverop
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
using FluentAssertions;
using JUS.Tool;
using JUS.Tool.Graphics.Converters;
using NUnit.Framework;
using Texim.Sprites;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JUS.Tests.Utils
{
    public class UtilsTests
    {
        [Test]
        public void DeserializeYamlTest()
        {
            var segment = new ImageSegment {
                TileIndex = 4,
                CoordinateX = -10,
                CoordinateY = 20,
                PaletteIndex = 0,
                Width = 32,
                Height = 16,
                HorizontalFlip = true,
                VerticalFlip = false,
            };

            var sprite = new Sprite { Width = 256, Height = 256 };
            sprite.Segments.Add(segment);

            string yaml = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Serialize(new List<Sprite> { sprite });

            List<Sprite> result = BinaryToDtx3.DeserializeYaml(yaml);

            result.Should().HaveCount(1);
            result[0].Width.Should().Be(256);
            result[0].Height.Should().Be(256);
            result[0].Segments.Should().HaveCount(1);

            IImageSegment expectedSegment = result[0].Segments[0];
            expectedSegment.Should().BeOfType<ImageSegment>();
            expectedSegment.TileIndex.Should().Be(4);
            expectedSegment.CoordinateX.Should().Be(-10);
            expectedSegment.CoordinateY.Should().Be(20);
            expectedSegment.PaletteIndex.Should().Be(0);
            expectedSegment.Width.Should().Be(32);
            expectedSegment.Height.Should().Be(16);
            expectedSegment.HorizontalFlip.Should().BeTrue();
            expectedSegment.VerticalFlip.Should().BeFalse();
        }
    }
}
