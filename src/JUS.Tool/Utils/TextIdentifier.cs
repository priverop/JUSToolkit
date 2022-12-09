// Copyright (c) 2021 SceneGate

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
namespace JUSToolkit.Utils
{
    /// <summary>
    /// Identifier methods for text files.
    /// </summary>
    public static class TextIdentifier
    {
        public static Dictionary<string, string> BinDictionary = new Dictionary<string, string>() {
            // { "tutorial0.bin", "Tutorial" },
            // { "tutorial1.bin", "Tutorial" },
            // { "tutorial2.bin", "Tutorial" },
            // { "tutorial3.bin", "Tutorial" },
            // { "tutorial4.bin", "Tutorial" },
            // { "tutorial5.bin", "Tutorial" },
            // { "tutorial.bin", "Tutorial" },

            // { "ability_t.bin"," " },
            { "bgm.bin", "Bgm" },
            { "chr_b_t.bin", "BtlChr" },
            { "chr_s_t.bin", "SuppChr" },
            { "clearlst.bin", "SimpleBin" },
            // { "commwin.bin"," " },
            { "demo.bin", "Demo" },
            { "infoname.bin", "SimpleBin" },
            // { "komatxt.bin"," " },
            { "location.bin", "Location" },
            { "piece.bin", "Piece" },
            // { "rulemess.bin"," " },
            { "stage.bin", "Stage" },
            { "title.bin", "SimpleBin" },
            { "pname.bin", "Pname" },
            // { "bin-InfoDeck"," " },

            // { "jquiz.bin"," " },
        };


        /// <summary>
        /// Returns the format name of the .bin file.
        /// </summary>
        /// <param name="fileName">The name of the file we want to check.</param>
        /// <returns>The format.</returns>
        public static string GetTextFormat(string fileName)
        {
            return BinDictionary[fileName];
        }
    }
}