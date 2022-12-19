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
using System;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between Pname format and BinaryFormat.
    /// </summary>
    public class Binary2Pname :
        IConverter<BinaryFormat, Pname>,
        IConverter<Pname, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// Converts BinaryFormat to Pname format.
        /// </summary>
        public Pname Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var pname = new Pname();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            pname.Count = reader.ReadInt32();

            for (int i = 0; i < pname.Count; i++) {
                pname.TextEntries.Add(JusText.ReadIndirectString(reader));
            }

            return pname;
        }

        /// <summary>
        /// Converts Pname format to BinaryFormat.
        /// </summary>
        public BinaryFormat Convert(Pname pname)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(SimpleBin.EntrySize * pname.Count + 0x04);

            writer.Write(pname.Count);
            foreach (string entry in pname.TextEntries) {
                JusText.WriteStringPointer(entry, writer, jit);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }
    }
}
