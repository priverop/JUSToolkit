// Copyright (c) 2024 SceneGate, Darkc0m, PRiverop

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
#nullable enable
using System;
using System.IO;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUSToolkit
{
    /// <summary>
    /// Table class for special characters of the font.
    /// </summary>
    public sealed class Table
    {
        private static readonly string TablePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "Resources", "table.tbl"));
        private static readonly object InstanceLock = new object();
        private static Table instance = null!;
        private Replacer replacer = null!;

        private Table()
        {
            using Stream stream = GetTableStream();
            LoadTable(stream);
        }

        /// <summary>
        /// Gets the unique instance of the Class. Singleton Pattern.
        /// </summary>
        /// <returns>The Table instance.</returns>
        public static Table Instance {
            get {
                if (instance == null) {
                    lock (InstanceLock) {
                        instance ??= new Table();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the Stream of the Table.
        /// </summary>
        /// <returns>The Stream of the Table.</returns>
        public Stream GetTableStream() =>
            DataStreamFactory.FromFile(TablePath, FileOpenMode.Read)
                ?? throw new FileNotFoundException("Cannot find table resource");

        /// <summary>
        /// Decode a string converting the special chars of the table (JAP->ESP).
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <returns>The decoded string.</returns>
        public string Decode(string text) => replacer.TransformBackward(text);

        /// <summary>
        /// Encode a string converting the special chars of the table (ESP->JAP).
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <returns>The encoded string.</returns>
        public string Encode(string text) => replacer.TransformForward(text);

        /// <summary>
        /// Fills up the Replacer with the table contents.
        /// </summary>
        /// <param name="stream">The stream of the table.</param>
        public void LoadTable(Stream stream)
        {
            replacer = new Replacer();
            var reader = new StreamReader(stream);

            while (!reader.EndOfStream) {
                string? line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) {
                    continue;
                }

                if (line[0] == '#') {
                    continue;
                }

                string[] fields = line.Split('=');
                replacer.Add(fields[0], fields[1]);
            }
        }
    }
}
