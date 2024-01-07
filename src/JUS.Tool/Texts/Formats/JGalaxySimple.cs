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
    /// Format for simple JGalaxy files.
    /// </summary>
    public class JGalaxySimple : IFormat
    {
        /// <summary>
        /// Size of mission.bin and battle.bin entries.
        /// </summary>
        public static readonly int EntrySize = 164;

        /// <summary>
        /// Initializes a new instance of the <see cref="JGalaxySimple"/> class.
        /// </summary>
        public JGalaxySimple()
        {
            Entries = new List<JGalaxyEntry>();
        }

        /// <summary>
        /// Gets or sets the number of <see cref="JGalaxySimple"/> entries.
        /// </summary>
        public int NumberOfEntries { get; set; }

        /// <summary>
        /// Gets or sets the list of text entries.
        /// </summary>
        public List<JGalaxyEntry> Entries { get; set; }
    }
}
