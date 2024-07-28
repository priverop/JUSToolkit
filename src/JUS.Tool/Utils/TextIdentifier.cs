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
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JUSToolkit.Texts.Converters;

namespace JUSToolkit.Utils
{
    /// <summary>
    /// Identifier methods for text files.
    /// </summary>
    public static class TextIdentifier
    {
        private static readonly Dictionary<string, Type[]> BinDictionary = new() {
            { "ability_t.bin", [typeof(Binary2Ability), typeof(Ability2Po)] },
            { "bgm.bin", [typeof(Binary2Bgm), typeof(Bgm2Po)] },
            { "chr_b_t.bin", [typeof(Binary2BtlChr), typeof(BtlChr2Po)] },
            { "chr_s_t.bin", [typeof(Binary2SuppChr), typeof(SuppChr2Po)] },
            { "clearlst.bin", [typeof(Binary2SimpleBin), typeof(SimpleBin2Po)] },
            { "commwin.bin", [typeof(Binary2Commwin), typeof(Commwin2Po)] },
            { "demo.bin", [typeof(Binary2Demo), typeof(Demo2Po)] },
            { "infoname.bin", [typeof(Binary2SimpleBin), typeof(SimpleBin2Po)] },
            { "komatxt.bin", [typeof(Binary2Komatxt), typeof(Komatxt2Po)] },
            { "location.bin", [typeof(Binary2Location), typeof(Location2Po)] },
            { "piece.bin", [typeof(Binary2Piece), typeof(Piece2Po)] },
            { "rulemess.bin", [typeof(Binary2Rulemess), typeof(Rulemess2Po)] },
            { "stage.bin", [typeof(Binary2Stage), typeof(Stage2Po)] },
            { "title.bin", [typeof(Binary2SimpleBin), typeof(SimpleBin2Po)] },
            { "pname.bin", [typeof(Binary2Pname), typeof(Pname2Po)] },
            { "tutorial.bin", [typeof(Binary2Tutorial), typeof(Tutorial2Po)] },
            { "tutorial0.bin", [typeof(Binary2Tutorial), typeof(Tutorial2Po)] },
            { "tutorial1.bin", [typeof(Binary2Tutorial), typeof(Tutorial2Po)] },
            { "tutorial2.bin", [typeof(Binary2Tutorial), typeof(Tutorial2Po)] },
            { "tutorial3.bin", [typeof(Binary2Tutorial), typeof(Tutorial2Po)] },
            { "tutorial4.bin", [typeof(Binary2Tutorial), typeof(Tutorial2Po)] },
            { "tutorial5.bin", [typeof(Binary2Tutorial), typeof(Tutorial2Po)] },
            { "battle.bin", [typeof(Binary2JGalaxySimple), typeof(JGalaxySimple2Po)] },
            { "mission.bin", [typeof(Binary2JGalaxySimple), typeof(JGalaxySimple2Po)] },
            { "jgalaxy.bin", [typeof(Binary2JGalaxyComplex), typeof(JGalaxyComplex2Po)] },
            { "jquiz.bin", [typeof(Binary2JQuiz), typeof(JQuiz2Po)] },
        };

        /// <summary>
        /// Returns the format name of the .bin file.
        /// </summary>
        /// <param name="fileName">The name of the file we want to check.</param>
        /// <returns>The format.</returns>
        public static Type[] GetTextFormat(string fileName) => IsInfoDeckFormat(fileName) ? [typeof(Binary2InfoDeck), typeof(InfoDeck2Po)] : BinDictionary[fileName];

        /// <summary>
        /// Checks if the fileName correspond with the InfoDeck format files.
        /// Example of correct format:  "bin-info-jump.bin" or "bin-deck-bb.bin".
        /// </summary>
        /// <param name="fileName">The name of the file we want to check.</param>
        /// <returns>True if the filename follows the pattern bin-info|deck-manga.bin.</returns>
        private static bool IsInfoDeckFormat(string fileName)
        {
            var regex = new Regex("^bin-.*-.*.bin$"); // ToDo: info or deck
            return regex.IsMatch(fileName);
        }

        // ToDo: Testing
    }
}
