// Copyright (c) 2022 Pablo Rivero

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
        IInitializer<Dictionary<string, int>>,
        IConverter<BinaryFormat, NodeContainerFormat>
    {
        /// <summary>
        /// Gets or sets the param to export the same image with multiple maps.
        /// </summary>
        public Dictionary<string, int> MultipleMaps { get; set; }

        /// <inheritdoc/>
        public void Initialize(Dictionary<string, int> maps)
        {
            MultipleMaps = maps;
        }

        /// <summary>
        /// Converts an ALAR container into a NodeContainerFormat with PNGs inside.
        /// </summary>
        /// <param name="alar">Alar file.</param>
        /// <returns><see cref="NodeContainerFormat"/> with the PNGs.</returns>
        public NodeContainerFormat Convert(BinaryFormat alar)
        {
            var transformedFiles = new NodeContainerFormat();

            // TODO: It is compressed?
            var alarVersion = Identifier.GetAlarVersion(alar);

            NodeContainerFormat alarNode = new NodeContainerFormat();

            // In the future we need to encapsulate this
            if (alarVersion.Major == 3) {
                alarNode = (Alar3)ConvertFormat.With<Binary2Alar3>(alar);
            } else if (alarVersion.Major == 2) {
                alarNode = (Alar2)ConvertFormat.With<Binary2Alar2>(alar);
            }

            // Iterate alar
            foreach (var child in Navigator.IterateNodes(alarNode.Root)) {
                if (Path.GetExtension(child.Name) == ".dig") {
                    var cleanName = Path.GetFileNameWithoutExtension(child.Name);
                    var childClone = new Node(cleanName, new BinaryFormat(child.Stream));

                    // Some containers (demo.aar) have a special type of .dig that needs
                    // to be compressed with 2 extra maps (we get 3 images with just one .dig)
                    // child.Name.Substring(2) removes the manga name
                    if (MultipleMaps is not null && MultipleMaps.ContainsKey(child.Name.Substring(2)))
                    {
                        string mangaName = child.Name.Substring(0, 2);

                        // _n_00.atm
                        using var atm_n = GetAtm(
                            GetSpecialMapName(mangaName, "n", child.Name.Substring(2)),
                            alarNode.Root.Children[0]);

                        if (atm_n is null) {
                            Console.WriteLine("Missing special n map file for: " + child.Name);
                        }

                        BinaryFormat stream_n = (BinaryFormat)childClone.GetFormatAs<BinaryFormat>().DeepClone();
                        var image_n = GetPNG(new Node(cleanName + "n", stream_n), atm_n, cleanName + "_n_");
                        if (image_n is not null) {
                            transformedFiles.Root.Add(image_n);
                        }

                        // _m_00.atm
                        using var atm_m = GetAtm(
                            GetSpecialMapName(mangaName, "m", child.Name.Substring(2)),
                            alarNode.Root.Children[0]);

                        if (atm_m is null) {
                            Console.WriteLine("Missing special m map file for: " + child.Name);
                        }

                        BinaryFormat stream_m = (BinaryFormat)childClone.GetFormatAs<BinaryFormat>().DeepClone();
                        var image_m = GetPNG(new Node(cleanName + "m", stream_m), atm_m, cleanName);
                        if (image_m is not null) {
                            transformedFiles.Root.Add(image_m);
                        }
                    }

                    // Get Map with the same name
                    using var atm = GetAtm(cleanName, alarNode.Root.Children[0]);
                    if (atm is null) {
                        Console.WriteLine("Missing map file for: " + child.Name);
                        continue;
                    }

                    var image = GetPNG(childClone, atm, cleanName);
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
            try {
                image = pixels.TransformWith<BinaryDig2Bitmap, Node>(atm);
            } catch (Exception ex) {
                Console.WriteLine("Exception detected in file " + cleanName + ": " + ex);
            }

            return image;
        }

        private Node GetAtm(string name, Node files)
        {
            var atm = Navigator.SearchNode(files, name + ".atm");

            if (atm is null) {
                Console.WriteLine("Atm doesn't exist: " + name + ".atm");
            }

            return atm;
        }
    }
}
