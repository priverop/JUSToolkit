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
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Komatxt format and Po.
    /// </summary>
    public class Komatxt2Po :
        IConverter<Komatxt, Po>,
        IConverter<Po, Komatxt>
    {
        /// <summary>
        /// Converts Komatxt format to Po.
        /// </summary>
        /// <param name="komatxt">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Komatxt komatxt)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (KomatxtEntry entry in komatxt.Entries) {
                po.Add(new PoEntry(entry.Name) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Unk2}",
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Komatxt format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Komatxt Convert(Po po)
        {
            var komatxt = new Komatxt();
            KomatxtEntry entry;
            string[] metadata;

            for (int i = 0; i < po.Entries.Count; i++) {
                entry = new KomatxtEntry();
                entry.Name = po.Entries[i].Text;

                metadata = JusText.ParseMetadata(po.Entries[i].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Unk2 = short.Parse(metadata[1]);

                komatxt.Entries.Add(entry);
            }

            return komatxt;
        }
    }
}
