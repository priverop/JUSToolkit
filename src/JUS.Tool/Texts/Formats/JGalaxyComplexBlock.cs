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
using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for blocks of the complex JGalaxy files.
    /// </summary>
    public class JGalaxyComplexBlock : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JGalaxyComplexBlock"/> class.
        /// </summary>
        /// <param name="numberOfEntries">Number of entries for the whole block.</param>
        /// <param name="startingPointer">Pointer of the start of the block.</param>
        public JGalaxyComplexBlock(short numberOfEntries, int startingPointer)
        {
            Entries = new List<JGalaxyEntry>();
            NumberOfEntries = numberOfEntries;
            StartPointer = startingPointer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JGalaxyComplexBlock"/> class.
        /// </summary>
        /// <param name="startingPointer">Pointer of the start of the block.</param>
        public JGalaxyComplexBlock(int startingPointer)
        {
            Entries = new List<JGalaxyEntry>();
            StartPointer = startingPointer;
        }

        /// <summary>
        /// Gets or sets the number of <see cref="JGalaxyComplexBlock"/> entries.
        /// </summary>
        public short NumberOfEntries { get; set; }

        /// <summary>
        /// Gets or sets the offset of the block start in the file.
        /// </summary>
        public int StartPointer { get; set; }

        /// <summary>
        /// Gets or sets the list of text entries.
        /// </summary>
        public List<JGalaxyEntry> Entries { get; set; }
    }
}
