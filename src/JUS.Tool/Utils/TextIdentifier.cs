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
using System.Text.RegularExpressions;

namespace JUSToolkit.Utils
{
    /// <summary>
    /// Identifier methods for text files.
    /// </summary>
    public static class TextIdentifier
    {
        private static readonly Dictionary<string, string> BinDictionary = new Dictionary<string, string>() {
            { "ability_t.bin", "Ability" },
            { "bgm.bin", "Bgm" },
            { "chr_b_t.bin", "BtlChr" },
            { "chr_s_t.bin", "SuppChr" },
            { "clearlst.bin", "SimpleBin" },
            { "demo.bin", "Demo" },
            { "infoname.bin", "SimpleBin" },
            { "komatxt.bin", "Komatxt" },
            { "location.bin", "Location" },
            { "piece.bin", "Piece" },
            { "rulemess.bin", "Rulemess" },
            { "stage.bin", "Stage" },
            { "title.bin", "SimpleBin" },
            { "pname.bin", "Pname" },
            { "tutorial.bin", "Tutorial" },
            { "tutorial0.bin", "Tutorial" },
            { "tutorial1.bin", "Tutorial" },
            { "tutorial2.bin", "Tutorial" },
            { "tutorial3.bin", "Tutorial" },
            { "tutorial4.bin", "Tutorial" },
            { "tutorial5.bin", "Tutorial" },
            { "battle.bin", "JGalaxySimple" },
            { "mission.bin", "JGalaxySimple" },
            { "jquiz.bin", "JQuiz" },
        };

        /// <summary>
        /// Returns the format name of the .bin file.
        /// </summary>
        /// <param name="fileName">The name of the file we want to check.</param>
        /// <returns>The format.</returns>
        public static string GetTextFormat(string fileName)
        {
            return IsInfoDeckFormat(fileName) ? "InfoDeck" : BinDictionary[fileName];
        }

        /// <summary>
        /// Checks if the fileName correspond with the InfoDeck format files.
        /// Example of correct format:  "bin-info-jump.bin" or "bin-deck-bb.bin".
        /// </summary>
        /// <param name="fileName">The name of the file we want to check.</param>
        /// <returns>True if the filename follows the pattern bin-info|deck-manga.bin.</returns>
        private static bool IsInfoDeckFormat(string fileName)
        {
            Regex regex = new Regex("^bin-.*-.*.bin$"); // ToDo: info or deck
            return regex.IsMatch(fileName);
        }

        // ToDo: Testing
    }
}
