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
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between BinInfoTitle and Po.
    /// </summary>
    public class BinInfoTitle2Po : IConverter<BinInfoTitle, Po>
    {
        /// <summary>
        /// Converts a BinaryInfoTitle Node to a Po Node.
        /// </summary>
        /// <param name="source">BinInfoTitle Node.</param>
        /// <returns>Po Node.</returns>
        public Po Convert(BinInfoTitle source)
        {
            var exportPo = new Po()
            {
                Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es")
                {
                    LanguageTeam = "TranScene",
                },
            };

            int i = 0;

            // Escribo las entradas del Po a partir de la lista de textos
            foreach (string entry in source.Text)
            {
                exportPo.Add(new PoEntry(entry) { Context = i.ToString() });
                i++;
            }

            return exportPo;
        }
    }
}
