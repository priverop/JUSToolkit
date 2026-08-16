// Copyright (c) 2022 Priverop

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using JUS.Tool.Containers;
using JUS.Tool.Graphics.Converters;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.BatchConverters
{
    /// <summary>
    /// Inserts a PNG into an Alar3.
    /// </summary>
    public class Png2Alar3 :
        IConverter<Alar, Alar>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Png2Alar3"/> class.
        /// </summary>
        /// <param name="image">PNG to insert.</param>
        /// <param name="digName">Name of the Dig.</param>
        /// <param name="atmName">Name of the atm.</param>
        /// <param name="insertTransparent">Label to add a transparent pixel in the image.</param>
        public Png2Alar3(Node image, string digName, string atmName, bool insertTransparent)
        {
            Image = image;
            DigName = digName;
            AtmName = atmName;
            TransparentTile = insertTransparent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2Alar3"/> class.
        /// </summary>
        /// <param name="image">PNG to insert.</param>
        /// <param name="digName">Name of the Dig.</param>
        /// <param name="atmName">Name of the atm.</param>
        public Png2Alar3(Node image, string digName, string atmName)
        {
            Image = image;
            DigName = digName;
            AtmName = atmName;
            TransparentTile = false;
        }

        /// <summary>
        /// Gets or sets the PNG we are inserting.
        /// </summary>
        public Node Image { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Dig of the image.
        /// </summary>
        public string DigName { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Atm of the image.
        /// </summary>
        public string AtmName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value indicating the transparent pixel mode.
        /// </summary>
        public bool TransparentTile { get; set; }

        /// <summary>
        /// Converts a <see cref="Node"/> (png file) to a <see cref="Alar"/> container.
        /// </summary>
        /// <param name="originalAlar">Original Alar3.</param>
        /// <returns><see cref="Alar"/>Alar3 with the PNG inserted.</returns>
        public Alar Convert(Alar originalAlar)
        {
            if (Path.GetExtension(Image.Name) != ".png") {
                throw new FormatException("Invalid png file");
            }

            // Obtaining the original Dig and Almt
            Node dig = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == DigName) ?? throw new FormatException("Dig doesn't exist: " + DigName);
            Node atm = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == AtmName) ?? throw new FormatException("Atm doesn't exist: " + AtmName);

            // Clone the nodes
            var dig_clone = (BinaryFormat)new BinaryFormat(dig.Stream!).DeepClone();
            var atm_clone = (BinaryFormat)new BinaryFormat(atm.Stream!).DeepClone();

            // Transform the PNG into the new Dig and Almt (we need the original dig + atm)
            var converter = new Png2DigAtm(new Node(dig.Name, dig_clone), new Node(atm.Name, atm_clone), true);

            NodeContainerFormat transformedFiles = converter.Convert(Image);

            originalAlar.InsertModification(transformedFiles);

            return originalAlar;
        }
    }
}
