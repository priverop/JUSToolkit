// Copyright (c) 2022 Pablo Rivero
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Collections.Generic;
using System.Linq;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Bin Format to Po Node.
    /// </summary>
    public class Bin2Po : IConverter<Bin, Po>
    {
        /// <summary>
        /// Converts Bin Node to Po Node.
        /// </summary>
        /// <param name="source">Bin Node.</param>
        /// <returns>Po Node.</returns>
        public Po Convert(Bin source)
        {
            var poExport = new Po {
                Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es") {
                    LanguageTeam = "TranScene",
                },
            };

            int i = 0;
            foreach (string sentence in from KeyValuePair<string, int> entry in source.Text
                                     let sentence = entry.Key
                                     select sentence) {
                string cleanSentence = string.IsNullOrEmpty(sentence) ? "<!empty>" : sentence;

                poExport.Add(new PoEntry(cleanSentence) { Context = i.ToString() });
                i++;
            }

            return poExport;
        }
    }
}
