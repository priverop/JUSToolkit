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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.BatchConverters
{
    /// <summary>
    /// Converts a bunch of PNGs to and Alar3.
    /// </summary>
    public class Alar2Png :
        IConverter<BinaryFormat, NodeContainerFormat>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alar2Png"/> class.
        /// </summary>
        /// <param name="maps">Dictionary with the maps.</param>
        public Alar2Png(Dictionary<string, int> maps)
        {
            MultipleMaps = maps;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alar2Png"/> class.
        /// </summary>
        public Alar2Png()
        {
        }

        /// <summary>
        /// Gets or sets the param to export the same image with multiple maps.
        /// </summary>
        public Dictionary<string, int> MultipleMaps { get; set; }

        /// <summary>
        /// Converts an ALAR container into a NodeContainerFormat with PNGs inside.
        /// </summary>
        /// <param name="alar">Alar file.</param>
        /// <returns><see cref="NodeContainerFormat"/> with the PNGs.</returns>
        public NodeContainerFormat Convert(BinaryFormat alar)
        {
            var transformedFiles = new NodeContainerFormat();

            // TODO: It is compressed?
            Version alarVersion = Identifier.GetAlarVersion(alar);

            var alarNode = new NodeContainerFormat();

            // In the future we need to encapsulate this
            if (alarVersion.Major == 3) {
                alarNode = alar.ConvertWith(new Binary2Alar3());
            } else if (alarVersion.Major == 2) {
                alarNode = alar.ConvertWith(new Binary2Alar2());
            }

            // Iterate alar
            foreach (Node child in Navigator.IterateNodes(alarNode.Root)) {
                if (Path.GetExtension(child.Name) == ".dig") {
                    string cleanName = Path.GetFileNameWithoutExtension(child.Name);
                    var childClone = new Node(cleanName, new BinaryFormat(child.Stream));
                    string mangaName = child.Name.Substring(0, 2);
                    string fileNumber = child.Name.Substring(2);

                    // Some containers (demo.aar) have a special type of .dig that needs
                    // to be compressed with 2 extra maps (we get 3 images with just one .dig)
                    // child.Name.Substring(2) removes the manga name
                    if (MultipleMaps?.ContainsKey(fileNumber) == true) {
                        // _n_00.atm
                        using Node atm_n = GetAtm(
                            GetSpecialMapName(mangaName, "n", fileNumber),
                            alarNode.Root.Children[0]);

                        if (atm_n is null) {
                            Console.WriteLine("Missing special n map file for: " + child.Name);
                        }

                        var stream_n = (BinaryFormat)childClone.GetFormatAs<BinaryFormat>().DeepClone();
                        Node image_n = GetPNG(new Node(Path.GetFileNameWithoutExtension(atm_n.Name), stream_n), atm_n, cleanName + "_n_");
                        if (image_n is not null) {
                            transformedFiles.Root.Add(image_n);
                        }

                        // _m_00.atm
                        using Node atm_m = GetAtm(
                            GetSpecialMapName(mangaName, "m", fileNumber),
                            alarNode.Root.Children[0]);

                        if (atm_m is null) {
                            Console.WriteLine("Missing special m map file for: " + child.Name);
                        }

                        var stream_m = (BinaryFormat)childClone.GetFormatAs<BinaryFormat>().DeepClone();
                        Node image_m = GetPNG(new Node(Path.GetFileNameWithoutExtension(atm_m.Name), stream_m), atm_m, cleanName);
                        if (image_m is not null) {
                            transformedFiles.Root.Add(image_m);
                        }
                    }

                    // Get Map with the same name
                    using Node atm = GetAtm(cleanName, alarNode.Root.Children[0]);
                    if (atm is null) {
                        Console.WriteLine("Missing map file for: " + child.Name);
                        continue;
                    }

                    Node image = GetPNG(childClone, atm, cleanName);
                    if (image is not null) {
                        transformedFiles.Root.Add(image);
                    }
                }
            }

            return transformedFiles;
        }

        /// <summary>
        /// Gets the name of the special map. Example: "bb_n_00.atm".
        /// </summary>
        private string GetSpecialMapName(string mangaName, string type, string fileNumber)
        {
            return mangaName + "_" + type + "_0" +
                MultipleMaps.GetValueOrDefault(fileNumber);
        }

        private Node GetPNG(Node pixels, Node atm, string cleanName)
        {
            Node image = null;
            var dig2Bitmap = new BinaryDig2Bitmap(atm);
            try {
                image = pixels.TransformWith(dig2Bitmap);
            } catch (Exception ex) {
                Console.WriteLine("Exception detected in file " + cleanName + ": " + ex);
            }

            return image;
        }

        private Node GetAtm(string name, Node files)
        {
            Node atm = Navigator.IterateNodes(files).First(n => n.Name == name + ".atm");

            if (atm is null) {
                Console.WriteLine("Atm doesn't exist: " + name + ".atm");
            }

            return atm;
        }
    }
}
