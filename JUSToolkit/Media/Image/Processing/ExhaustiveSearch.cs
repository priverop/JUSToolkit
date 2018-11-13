//
// ExhaustiveSearch.cs
//
// Author:
//       Benito Palacios Sanchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sanchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Texim.Media.Image.Processing
{
    using System.Drawing;

    public class ExhaustivePaletteSearch : NearestNeighbour<Color>
    {
        public override void Initialize(Color[] vertex)
        {
            this.vertex = vertex;
        }

        /// <summary>
        /// Get the palette index of the nearest color by using exhaustive search.
        /// </summary>
        /// <returns>The nearest color palette index.</returns>
        /// <param name="color">Color to get its nearest palette color.</param>
        public override int Search(Color color)
        {
            // Set the largest distance and a null index
            double minDistance = (255 * 255) + (255 * 255) + (255 * 255) + 1;
            int nearestColor = -1;

            // FUTURE: Implement "Approximate Nearest Neighbors in Non-Euclidean Spaces" algorithm or
            // k-d tree if it's computing CIE76 color difference
            for (int i = 0; i < this.vertex.Length && minDistance > 0; i++) {
                // Since we only want the value to compare, it is faster to not computer the squared root
                double distance = color.GetDistanceSquared(vertex[i]);
                if (distance < minDistance) {
                    minDistance = distance;
                    nearestColor = i;
                }
            }

            return nearestColor;
        }
    }
}

