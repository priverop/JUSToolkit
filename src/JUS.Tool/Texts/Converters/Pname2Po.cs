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
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Pname format and Po.
    /// </summary>
    public class Pname2Po :
        IConverter<Pname, Po>,
        IConverter<Po, Pname>
    {
        /// <summary>
        /// Converts Pname format to Po.
        /// </summary>
        /// <param name="pname">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Pname pname)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (string entry in pname.TextEntries) {
                po.Add(new PoEntry(entry) {
                    Context = $"{i++}",
                });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Pname format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Pname Convert(Po po)
        {
            var pname = new Pname();

            pname.Count = po.Entries.Count;
            foreach (PoEntry entry in po.Entries) {
                pname.TextEntries.Add(entry.Text);
            }

            return pname;
        }
    }
}
