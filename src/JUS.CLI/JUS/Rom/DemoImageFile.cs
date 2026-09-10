// Copyright (c) 2024 Priverop

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
using System.Text.RegularExpressions;
using JUS.Tool.BatchConverters;
using JUS.Tool.Containers.Converters;
using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.FileSystem;

namespace JUS.CLI.JUS.Rom
{
    /// <summary>
    /// Strategy Pattern: Interface for rom importing logic.
    /// </summary>
    public class DemoImageFile : IFileImportStrategy
    {
        private static readonly Dictionary<char, char> SpecialDigNumbers = new() {
            { '0', '3' },
            { '1', '5' },
            { '2', '7' },
            { '3', '9' },
        };

        private static readonly Regex DemoPattern = new(@"^demo-.*\.png$");

        /// <inheritdoc/>
        public bool Matches(string filename)
        {
            return DemoPattern.IsMatch(filename);
        }

        /// <inheritdoc/>
        public void Import(Node gameNode, List<Node> files)
        {
            string containerPath = "/demo/demo.aar";

            ProcessContainer(gameNode, files, containerPath);
        }

        private static void ProcessContainer(Node gameNode, List<Node> filesToInsert, string containerPath)
        {
            Node originalAlar = Navigator.GetNode(gameNode, $"/root/data{containerPath}") ?? throw new FormatException($"Container not found /root/data{containerPath}");
            _ = originalAlar.TransformWith<Binary2Alar3>();

            foreach (Node fileToInsert in filesToInsert) {
                fileToInsert.Name = StringFunctions.GetDemoName(fileToInsert.Name);
                // Ignore the _n_ and _m_, because we obtain them later with the original
                if (!IsSpecialDemoNM(fileToInsert.Name)) {
                    IConverter image2Alar3;

                    string digName = GetDemoDigName(fileToInsert.Name) + ".dig";
                    string atmName = Path.GetFileNameWithoutExtension(fileToInsert.Name) + ".atm";

                    // Special demo (3, 5, 7, 9) needs 3 atm, 3 pngs and 1 dig
                    // The rest (non special 2, 4, 6, title..., opening or sel) are 1 dig = 1 atm = 1 png
                    if (IsSpecialDemo(fileToInsert.Name) && !fileToInsert.Name.Contains("opening") && !fileToInsert.Name.Contains("sel01")) {
                        string[] atms = GetSpecialAtms(atmName);
                        Node[] pngs = GetSpecialPngs(fileToInsert);
                        image2Alar3 = new Demo2Alar3(pngs, digName, atms, true);
                    } else {
                        image2Alar3 = new Png2Alar3(fileToInsert, digName, atmName, true);
                    }

                    _ = originalAlar.TransformWith(image2Alar3);
                    Console.WriteLine($"File replaced: /root/data{containerPath}/{fileToInsert.Name}");
                }
            }
            _ = originalAlar.TransformWith(new Alar3ToBinary());
        }

        /// <summary>
        /// Returns true if the png is a Special Demo Image.
        /// </summary>
        /// <param name="pngName">The name of the Png we are importing.</param>
        /// <returns>True if it's a special demo image (03, 05, 07, 09, _m_ or _n_). False otherwise.</returns>
        private static bool IsSpecialDemo(string pngName) =>
            pngName.Contains("_03") ||
            pngName.Contains("_05") ||
            pngName.Contains("_07") ||
            pngName.Contains("_09") ||
            pngName.Contains("_m_") ||
            pngName.Contains("_n_");

        /// <summary>
        /// Returns true if the png is a _m_ or _n_ Special Demo Image.
        /// </summary>
        /// <param name="pngName">The name of the Png we are importing.</param>
        /// <returns>True if it's a special demo image (_m_ or _n_). False otherwise.</returns>
        private static bool IsSpecialDemoNM(string pngName) =>
            pngName.Contains("_m_") ||
            pngName.Contains("_n_");

        /// <summary>
        /// Obtains the name of the corresponding Dig.
        /// </summary>
        /// <remarks>
        /// bb_00.png => bb_00.dig
        /// bb_m_00.png => bb_03.dig
        /// bb_n_00.png => bb_03.dig
        /// bb_m_01.png => bb_05.dig
        /// bb_n_01.png => bb_05.dig
        /// bb_m_02.png => bb_07.dig
        /// bb_n_02.png => bb_07.dig
        /// bb_m_03.png => bb_09.dig
        /// bb_n_03.png => bb_09.dig.
        /// </remarks>
        private static string GetDemoDigName(string pngName)
        {
            // SpecialChar is the 4th character of the demo png name.
            // jj_m_00.png => m
            // jj_n_00.png => n
            // jj_03.png => not n nor m, so it's not a special one
            if (pngName.Contains("_m_") || pngName.Contains("_n_")) {
                // jj_m_01.png => 1
                char number = pngName[6];
                string manga = pngName[..2];
                return manga + "_0" + SpecialDigNumbers.GetValueOrDefault(number);
            } else {
                return Path.GetFileNameWithoutExtension(pngName);
            }
        }

        /// <summary>
        /// Constructs the names of the _m_ and _n_ files for the given base node name and extension.
        /// </summary>
        /// <param name="nodeName">The base name of the node ("bb_03.png").</param>
        /// <param name="extension">The file extension (".png", ".atm").</param>
        /// <returns>An array of strings containing the names of the base, _m_, and _n_ files.</returns>
        private static string[] GetSpecialFileNames(string nodeName, string extension)
        {
            string manga = nodeName[..2]; // "bb"
            char number = nodeName[4]; // '3'

            if (!SpecialDigNumbers.Any(kv => kv.Value == number)) {
                throw new InvalidOperationException($"Value {number} is not found in SpecialDigNumbers.");
            }

            char specialNumber = SpecialDigNumbers.First(kv => kv.Value == number).Key;

            string nameOfMFile = $"{manga}_m_0{specialNumber}{extension}";
            string nameOfNFile = $"{manga}_n_0{specialNumber}{extension}";

            return [nodeName, nameOfMFile, nameOfNFile];
        }

        /// <summary>
        /// Returns an array of Nodes with the given PNG Node, the _m_ and _n_ Nodes.
        /// </summary>
        /// <remarks>
        /// bb_03.png => [bb_03.png, bb_m_00.png, bb_n_00.png]
        /// bb_05.png => [bb_05.png, bb_m_01.png, bb_n_01.png]
        /// bb_07.png => [bb_07.png, bb_m_02.png, bb_n_02.png]
        /// bb_09.png => [bb_09.png, bb_m_03.png, bb_n_03.png].
        /// </remarks>
        private static Node[] GetSpecialPngs(Node png)
        {
            string[] fileNames = GetSpecialFileNames(png.Name, ".png");

            Node mNode = png.Parent!.Children["demo-" + fileNames[1]] ??
                    throw new FormatException("Special m file not found: " + fileNames[1]);
            Node nNode = png.Parent.Children["demo-" + fileNames[2]] ??
                    throw new FormatException("Special nfile not found: " + fileNames[2]);

            return [png, mNode, nNode];
        }

        /// <summary>
        /// Returns an array of strings with the names of the given ATM file, the _m_, and the _n_ files.
        /// </summary>
        /// <remarks>
        /// bb_03.atm => [bb_03.atm, bb_m_00.atm, bb_n_00.atm]
        /// bb_05.atm => [bb_05.atm, bb_m_01.atm, bb_n_01.atm]
        /// bb_07.atm => [bb_07.atm, bb_m_02.atm, bb_n_02.atm]
        /// bb_09.atm => [bb_09.atm, bb_m_03.atm, bb_n_03.atm].
        /// </remarks>
        private static string[] GetSpecialAtms(string atm)
        {
            return GetSpecialFileNames(atm, ".atm");
        }
    }
}
