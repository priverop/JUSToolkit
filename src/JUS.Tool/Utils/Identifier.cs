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
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Utils
{
    /// <summary>
    /// Compression auxiliar methods.
    /// </summary>
    public static class Identifier
    {
        /// <summary>
        /// Returns the version of the Alar file.
        /// </summary>
        /// <param name="file">The File we want to check.</param>
        /// <returns>The version.</returns>
        public static Version GetAlarVersion(BinaryFormat file)
        {
            return GetAlarVersion(file.Stream);
        }

        /// <summary>
        /// Returns the version of the Alar stream.
        /// </summary>
        /// <param name="stream">The stream we want to check.</param>
        /// <returns>The version.</returns>
        public static Version GetAlarVersion(DataStream stream)
        {
            var reader = new DataReader(stream);
            stream.Position = 4;
            var majorVersion = reader.ReadByte();
            var minorVersion = reader.ReadByte();
            stream.Position = 0;

            return new Version(majorVersion, minorVersion);
        }
    }
}