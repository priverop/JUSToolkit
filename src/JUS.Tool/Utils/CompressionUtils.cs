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
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Utils
{
    /// <summary>
    /// Compression auxiliar methods.
    /// </summary>
    public static class CompressionUtils
    {
        /// <summary>
        /// Returns true if the Node is compressed.
        /// </summary>
        /// <param name="node">The Node we want to check.</param>
        /// <returns>The result.</returns>
        public static bool IsCompressed(Node node)
        {
            return IsCompressed(node.Stream);
        }

        /// <summary>
        /// Returns true if the DataStream is compressed.
        /// </summary>
        /// <param name="stream">The Stream we want to check.</param>
        /// <returns>The result.</returns>
        public static bool IsCompressed(DataStream stream)
        {
            var reader = new DataReader(stream);
            stream.Position = 0;
            var result = reader.ReadString(4) == "DSCP";
            stream.Position = 0;

            return result;
        }
    }
}