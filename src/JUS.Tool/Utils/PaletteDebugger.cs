// Copyright (c) 2025 Priverop

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
using System.IO;
using System.Text;
using Texim.Pixels;

namespace JUSToolkit.Utils
{
    /// <summary>
    /// Utils to help debugging pixels and palettes.
    /// </summary>
    public static class PaletteDebugger
    {
        /// <summary>
        /// Groups the pixels by their color info (color index in palette, alpha/transparency, palette index).
        /// </summary>
        /// <param name="pixels">The pixels of the image.</param>
        public static void GroupAndDisplayPixels(IndexedPixel[] pixels)
        {
            var pixelGroups = new Dictionary<string, int>();

            for (int i = 0; i < pixels.Length; i++) {
                IndexedPixel pixel = pixels[i];
                string key = $"{pixel.Index},{pixel.Alpha},{pixel.PaletteIndex}";

                if (pixelGroups.ContainsKey(key)) {
                    pixelGroups[key]++;
                } else {
                    pixelGroups[key] = 1;
                }
            }

            foreach (KeyValuePair<string, int> group in pixelGroups) {
                string[] parts = group.Key.Split(',');
                Console.WriteLine($"You have {group.Value} pixels with Index: {parts[0]}, Alpha: {parts[1]}, P: {parts[2]}");
            }
        }
    }
}
