// Copyright (c) 2023 Priverop

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
    /// Converts between JGalaxySimple format and BinaryFormat.
    /// </summary>
    // public class Binary2JGalaxySimple :
    //     IConverter<BinaryFormat, JGalaxySimple>,
    //     IConverter<JGalaxySimple, BinaryFormat>
    // {
    //     /// <summary>
    //     /// Converts BinaryFormat to JGalaxySimple format.
    //     /// </summary>
    //     /// <param name="source">BinaryFormat to convert.</param>
    //     /// <returns>Text format.</returns>
    //     /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
    //     public JGalaxySimple Convert(BinaryFormat source)
    //     {
    //         if (source == null) {
    //             throw new ArgumentNullException(nameof(source));
    //         }

    //         var jgalaxy = new JGalaxySimple();
    //         var reader = new DataReader(source.Stream) {
    //             DefaultEncoding = JusText.JusEncoding,
    //         };

    //         reader.Stream.Position = 0x00;
    //         jgalaxy.NumberOfEntries = reader.ReadInt32();

    //         for (int i = 0; i < jgalaxy.NumberOfEntries; i++) {
    //             jgalaxy.TextEntries.Add(reader.ReadBytes(JGalaxySimple.EntrySize));
    //         }

    //         return jgalaxy;
    //     }

    //     /// <summary>
    //     /// Converts JGalaxySimple format to BinaryFormat.
    //     /// </summary>
    //     /// <param name="jgalaxy">TextFormat to convert.</param>
    //     /// <returns>BinaryFormat.</returns>
    //     public BinaryFormat Convert(JGalaxySimple jgalaxy)
    //     {
    //         var bin = new BinaryFormat();
    //         var writer = new DataWriter(bin.Stream) {
    //             DefaultEncoding = JusText.JusEncoding,
    //         };

    //         writer.Write(jgalaxy.NumberOfEntries);

    //         foreach (byte[] entry in jgalaxy.TextEntries) {
    //             writer.Write(entry);
    //         }

    //         return bin;
    //     }
    // }
}
