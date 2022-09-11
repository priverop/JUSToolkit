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
using System.IO;
using JUSToolkit.Containers;
using JUSToolkit.Containers.Converters;
using JUSToolkit.Graphics;
using JUSToolkit.Graphics.Converters;
using JUSToolkit.Utils;
using Texim.Compressions.Nitro;
using Texim.Formats;
using Texim.Images;
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
            if (alarVersion.Major == 3)
            {
                alarNode = (Alar3)ConvertFormat.With<Binary2Alar3>(alar);
            }
            else if (alarVersion.Major == 2)
            {
                alarNode = (Alar2)ConvertFormat.With<Binary2Alar2>(alar);
            }

            // Iterate NCF
            foreach (var child in Navigator.IterateNodes(alarNode.Root))
            {
                if (Path.GetExtension(child.Name) == ".dig")
                {
                    var cleanName = Path.GetFileNameWithoutExtension(child.Name);

                    using var atm = GetAtm(cleanName, alarNode.Root.Children[0]);
                    if (atm is null) {
                        Console.WriteLine("Missing map file for: " + child.Name);
                        continue;
                    }

                    // We transform the dig+atm (with compression)
                    try {
                        var image = new Node(cleanName, new BinaryFormat(child.Stream))
                        .TransformWith<BinaryDig2Bitmap, Node>(atm);
                        transformedFiles.Root.Add(image);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception detected in file " + cleanName + ": " + ex);
                    }
                }
            }

            return transformedFiles;
        }

        private Node GetAtm(string name, Node files)
        {
            var atm = Navigator.SearchNode(files, name + ".atm");

            if (atm is null)
            {
                Console.WriteLine("Atm doesn't exist: " + name + ".atm");
            }

            return atm;
        }
    }
}
