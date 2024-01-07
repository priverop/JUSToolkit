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
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between JGalaxyComplex format and Po.
    /// </summary>
    public class JGalaxyComplex2Po :
        IConverter<JGalaxyComplex, Po>,
        IConverter<Po, JGalaxyComplex>
    {
        /// <summary>
        /// Converts JGalaxyComplex format to Po.
        /// </summary>
        /// <param name="jgalaxy">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(JGalaxyComplex jgalaxy)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            int blockCount = 0;
            foreach (JGalaxyComplexBlock block in jgalaxy.Blocks) {
                po.Add(new PoEntry("<!Don't remove>") {
                    Context = $"{blockCount++}",
                    ExtractedComments = $"{block.StartPointer} - {block.NumberOfEntries}",
                });
                foreach (JGalaxyEntry entry in block.Entries) {
                    po.Add(new PoEntry(JusText.CleanString(entry.Description)) {
                        Context = $"{i++}",
                        ExtractedComments = $"{entry.EntrySize}-{System.Convert.ToBase64String(entry.Unknown)}",
                    });
                }
            }

            return po;
        }

        /// <summary>
        /// Converts Po to JGalaxyComplex format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public JGalaxyComplex Convert(Po po)
        {
            var jgalaxy = new JGalaxyComplex();

            int blockCount = 0;
            int i = 0;
            foreach (PoEntry entry in po.Entries) {
                // New block found
                if (entry.Text.Equals("<!Don't remove>")) {
                    if (i != 0) {
                        blockCount++;
                    }

                    string[] blockMetadata = JusText.ParseMetadata(entry.ExtractedComments);
                    int startPointer = int.Parse(blockMetadata[0]);
                    short numberOfEntries = short.Parse(blockMetadata[1]);

                    jgalaxy.Blocks[blockCount] = new JGalaxyComplexBlock(numberOfEntries, startPointer);

                    continue;
                }

                i++;

                var jgalaxyEntry = new JGalaxyEntry {
                    Description = entry.Text,
                };

                string[] metadata = JusText.ParseMetadata(entry.ExtractedComments);
                jgalaxyEntry.EntrySize = int.Parse(metadata[0]);
                jgalaxyEntry.Unknown = System.Convert.FromBase64String(metadata[1]);

                jgalaxy.Blocks[blockCount].Entries.Add(jgalaxyEntry);
            }

            return jgalaxy;
        }
    }
}
