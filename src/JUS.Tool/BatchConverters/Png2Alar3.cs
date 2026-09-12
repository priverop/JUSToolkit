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
            Images = [image];
            DigName = digName;
            AtmNames = [atmName];
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
            Images = [image];
            DigName = digName;
            AtmNames = [atmName];
            TransparentTile = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Png2Alar3"/> class for
        /// multiple PNGs sharing the same Dig.
        /// <param name="images">PNGs to insert.</param>
        /// <param name="digName">Name of the shared Dig.</param>
        /// <param name="atmNames">Name of each atm.</param>
        /// <param name="insertTransparent">Label to add a transparent pixel in the image.</param>
        public Png2Alar3(Node[] images, string digName, string[] atmNames, bool insertTransparent)
        {
            Images = images;
            DigName = digName;
            AtmNames = atmNames;
            TransparentTile = insertTransparent;
        }

        /// <summary>
        /// Gets or sets the PNGs we are inserting.
        /// </summary>
        public Node[] Images { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Dig of the image.
        /// </summary>
        public string DigName { get; set; }

        /// <summary>
        /// Gets or sets the original name of the Atm of each image.
        /// </summary>
        public string[] AtmNames { get; set; }

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
            foreach (Node image in Images) {
                if (Path.GetExtension(image.Name) != ".png") {
                    throw new FormatException("Invalid png file");
                }
            }

            // Obtaining the original Dig and Altm
            Node dig = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == DigName) ?? throw new FormatException("Dig doesn't exist: " + DigName);

            // Clone the nodes
            var dig_clone = (BinaryFormat)new BinaryFormat(dig.Stream).DeepClone();

            var atm_clones = new Node[AtmNames.Length];
            for (int i = 0; i < AtmNames.Length; i++) {
                Node atm = Navigator.IterateNodes(originalAlar.Root).First(n => n.Name == AtmNames[i]) ?? throw new FormatException("Atm doesn't exist: " + AtmNames[i]);
                var atm_clone = (BinaryFormat)new BinaryFormat(atm.Stream).DeepClone();
                atm_clones[i] = new Node(atm.Name, atm_clone);
            }

            // Transform the PNGs into the new Dig and Altms (we need the original dig + atms)
            var converter = new Png2DigAtm(new Node(dig.Name, dig_clone), atm_clones, true);

            NodeContainerFormat transformedFiles = converter.Convert(Images);

            originalAlar.InsertModification(transformedFiles);

            return originalAlar;
        }
    }
}
