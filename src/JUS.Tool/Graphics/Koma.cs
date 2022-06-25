// Copyright (c) 2021 SceneGate

using System.Collections.ObjectModel;
using Yarhl.FileFormat;

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
namespace JUSToolkit.Graphics
{
    /// <summary>
    /// Sprite Format - A Koma is a Collection of KomaElements.
    /// </summary>
    public class Koma : Collection<KomaElement>, IFormat
    {
        /// <summary>
        /// Gets the table with the Game names. From ARM9 pointers at 0x0209E840.
        /// </summary>
        public static string[] NameTable { get; } = new[] {
            null, "es", "is", "ig",
            "tr", "ct", "gt", "kn",
            "kk", "cb", "oj", "nb",
            "sk", "tc", "jj", "sd",
            "ss", "tz", "to", "dg",
            "dn", "ds", "tl", "db",
            "na", "nk", "hh", "pj",
            "bu", "bc", "bl", "hs",
            "hk", "bb", "nn", "mo",
            "mr", "yo", "yh", "rk",
            "rb", "op", "dt",
        };
    }
}
