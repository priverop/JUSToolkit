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
using System;
using JUSToolkit.Graphics.Converters;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Decompression algorithm for LZSS using CUE's implementation.
    /// </summary>
    public class LzssDecompression :
        IConverter<IBinary, BinaryFormat>
    {
        /// <summary>
        /// Decompress a LZSS compressed IBinary stream.
        /// </summary>
        /// <param name="source">The compressed IBinary stream with LZSS.</param>
        /// <returns>The decompressed stream.</returns>
        public BinaryFormat Convert(IBinary source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var decompressedStream = IsCompressed(source.Stream) ? Convert(source.Stream) : source.Stream;

            return new BinaryFormat(decompressedStream);
        }

        /// <summary>
        /// Decompress a LZSS compressed DataStream.
        /// </summary>
        /// <param name="source">The compressed DataStream with LZSS.</param>
        /// <returns>The decompressed DataStream.</returns>
        public DataStream Convert(DataStream source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            // Discard the first 4 bytes of the header (the DSCP magic ID)
            return LzssUtils.Lzss(new DataStream(source, 4, source.Length - 4), "-d");
        }

        private bool IsCompressed(DataStream stream)
        {
            var reader = new DataReader(stream);
            stream.Position = 0;

            return reader.ReadString(4) == "DSCP";
        }
    }
}
