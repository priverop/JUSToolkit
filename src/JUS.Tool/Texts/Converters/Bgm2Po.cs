// Copyright (c) 2021 Darkc0m

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
using System.Collections.Generic;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Bgm and Po.
    /// </summary>
    public class Bgm2Po :
        IConverter<Bgm, Po>,
        IConverter<Po, Bgm>
    {
        /// <summary>
        /// Converts Bgm format to Po.
        /// </summary>
        /// <param name="bgm">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Bgm bgm)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            foreach (BgmEntry entry in bgm.Entries) {
                po.Add(new PoEntry(entry.Title) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Unk2}-{entry.Icon}",
                });
                string description = $"{entry.Desc1}\n{entry.Desc2}\n{entry.Desc3}";
                po.Add(new PoEntry(description.TrimEnd()) { Context = $"{i++}", });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Bgm format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Bgm Convert(Po po)
        {
            var bgm = new Bgm();
            BgmEntry entry;
            List<string> description;
            string[] metadata;

            bgm.Count = po.Entries.Count / 2;

            for (int i = 0; i < bgm.Count; i++) {
                entry = new BgmEntry();
                entry.Title = Table.Instance.Encode(po.Entries[i * 2].Text);

                description = JusText.SplitStringToList(Table.Instance.Encode(po.Entries[(i * 2) + 1].Text), '\n', 3);
                entry.Desc1 = description[0];
                entry.Desc2 = description[1];
                entry.Desc3 = description[2];

                metadata = JusText.ParseMetadata(po.Entries[i * 2].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Unk2 = short.Parse(metadata[1]);
                entry.Icon = int.Parse(metadata[2]);

                bgm.Entries.Add(entry);
            }

            return bgm;
        }
    }
}
