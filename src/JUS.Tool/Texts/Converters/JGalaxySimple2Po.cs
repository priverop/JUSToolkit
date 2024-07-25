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
    /// Converts between JGalaxySimple format and Po.
    /// </summary>
    public class JGalaxySimple2Po :
        IConverter<JGalaxySimple, Po>,
        IConverter<Po, JGalaxySimple>
    {
        /// <summary>
        /// Converts JGalaxySimple format to Po.
        /// </summary>
        /// <param name="jgalaxy">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(JGalaxySimple jgalaxy)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            foreach (JGalaxyEntry entry in jgalaxy.Entries) {
                po.Add(new PoEntry(JusText.CleanString(entry.Description)) {
                    Context = $"{i++}",
                    ExtractedComments = System.Convert.ToBase64String(entry.Unknown),
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to JGalaxySimple format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public JGalaxySimple Convert(Po po)
        {
            var jgalaxy = new JGalaxySimple {
                NumberOfEntries = po.Entries.Count,
            };

            foreach (PoEntry entry in po.Entries) {
                var jgalaxyEntry = new JGalaxyEntry {
                    Description = Table.Instance.Encode(entry.Text),
                    Unknown = System.Convert.FromBase64String(entry.ExtractedComments),
                };

                jgalaxy.Entries.Add(jgalaxyEntry);
            }

            return jgalaxy;
        }
    }
}
