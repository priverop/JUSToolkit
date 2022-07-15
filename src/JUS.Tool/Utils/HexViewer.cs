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
using System.IO;
using System.Text;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Hex and ascii viewer by Pleonex.
    /// </summary>
    public static class HexViewer
    {
        /// <summary>
        /// Transforms the Stream into a table so we can display it.
        /// </summary>
        /// <param name="stream">The Stream we want to display.</param>
        /// <param name="startPosition">The position where we want to start to read.</param>
        /// <param name="length">The length of the stream.</param>
        /// <returns>The string.</returns>
        public static string DumpStreamToHexString(Stream stream, long startPosition, int length)
        {
            const int BytesPerRow = 0x10;
            const char horizontalBar = '─'; // or '|' for non-Unicode terminals
            const char verticalBar = '│'; // or '-' for non-Unicode terminals
            const char crossBar = '┼'; // or '|' for non-Unicode terminals

            byte[] buffer = new byte[length];
            int read = stream.Read(buffer);

            var content = new StringBuilder();
            var textLineBuilder = new StringBuilder();

            // Header
            content.AppendFormat("Offset   {0} ", verticalBar);
            for (int i = 0; i < BytesPerRow; i++) {
                content.AppendFormat("{0:X2} ", i);
            }

            content.AppendFormat("{0} ASCII\n", verticalBar);
            content.Append(new string(horizontalBar, 9));
            content.Append(crossBar);
            content.Append(new string(horizontalBar, 1 + (BytesPerRow * 3)));
            content.Append(crossBar);
            content.AppendLine(new string(horizontalBar, 1 + (BytesPerRow * 2)));

            // For each line: offset, hex content and text content
            content.AppendFormat("{0:X8} {1} ", startPosition, verticalBar);
            for (int i = 0; i < read; i++) {
                char ch = (buffer[i] >= 0x21 && buffer[i] <= 0x7F) ? (char)buffer[i] : '.';
                textLineBuilder.AppendFormat("{0} ", ch);

                if (i != 0 && ((i + 1) % BytesPerRow == 0)) {
                    content.AppendFormat("{0:X2} {2} {1}\n", buffer[i], textLineBuilder.ToString(), verticalBar);
                    content.AppendFormat("{0:X8} {1} ", startPosition + (i + 1), verticalBar);
                    textLineBuilder.Clear();
                } else {
                    content.AppendFormat("{0:X2} ", buffer[i]);
                }
            }

            return content.ToString();
        }
    }
}