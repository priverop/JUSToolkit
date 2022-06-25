// Copyright (c) 2022 SceneGate

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
using Texim.Sprites;
using Yarhl.FileFormat;

namespace Texim.Games.JumpUltimateStars
{
    /// <summary>
    /// Image Format - KShape defines the geometric shape of the Koma.
    /// </summary>
    public class KShapeSprites : IFormat
    {
        private readonly Dictionary<(int, int), Sprite> sprites = new ();

        /// <summary>
        /// Adds a new sprite to the collection.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="element">Element.</param>
        /// <param name="sprite">Sprite.</param>
        public void AddSprite(int group, int element, Sprite sprite) => sprites[(group, element)] = sprite;

        /// <summary>
        /// Gets the sprite with the specified group and element.
        /// </summary>
        /// <param name="group">Group.</param>
        /// <param name="element">Element.</param>
        /// <returns>Sprite.</returns>
        public Sprite GetSprite(int group, int element) => sprites[(group, element)];
    }
}