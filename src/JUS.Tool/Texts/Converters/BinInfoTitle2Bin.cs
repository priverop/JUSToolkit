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
using System.Text;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between a <see cref="BinaryFormat"/> and a <see cref="BinInfoTitle"/>.
    /// </summary>
    public class BinInfoTitle2Bin : IConverter<BinInfoTitle, BinaryFormat>
    {
        /// <summary>
        /// Converts a BinInfoTitle to a BinaryFormat.
        /// </summary>
        /// <param name="source">BinInfoTitle Node.</param>
        /// <returns>BinaryFormat Node.</returns>
        public BinaryFormat Convert(BinInfoTitle source)
        {
            var bin = new BinaryFormat();

            var writer = new DataWriter(bin.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding(932),
            };

            foreach (int pointer in source.Pointers)
            {
                writer.WriteOfType((short)pointer);
            }

            foreach (string text in source.Text)
            {
                writer.Write(text);
            }

            return bin;
        }
    }
}
