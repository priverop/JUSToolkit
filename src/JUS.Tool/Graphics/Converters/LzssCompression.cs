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
using System;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Compression algorithm for LZSS using CUE's implementation.
    /// </summary>
    public class LzssCompression :
        IConverter<IBinary, BinaryFormat>
    {
        /// <summary>
        /// Compress a decompressed stream with LZSS and adds the DSCP header.
        /// </summary>
        /// <param name="source">The stream to compress.</param>
        /// <returns>The compressed stream.</returns>
        public BinaryFormat Convert(IBinary source)
        {
            ArgumentNullException.ThrowIfNull(source);

            DataStream decompressedStream = Convert(source.Stream);

            return new BinaryFormat(decompressedStream);
        }

        /// <summary>
        /// Compress a DataStream with LZSS.
        /// </summary>
        /// <param name="source">The DataStream to compress.</param>
        /// <returns>The compressed DataStream.</returns>
        public DataStream Convert(DataStream source)
        {
            ArgumentNullException.ThrowIfNull(source);

            DataStream compressed = LzssUtils.Lzss(source, "-evn");

            // Write the DSCP magic ID header
            var memoryStream = new DataStream();

            memoryStream.Seek(0);
            memoryStream.Write(Encoding.ASCII.GetBytes("DSCP"), 0, 4);

            compressed.WriteTo(memoryStream);

            return memoryStream;
        }
    }
}
