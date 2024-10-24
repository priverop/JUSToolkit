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
    /// Helper methods for strings.
    /// </summary>
    public static class StringFunctions
    {
        /// <summary>
        /// Remove the first two words separated by dashes "x-y-".
        /// </summary>
        /// <param name="nameWithPattern">The string containing potentially "menu-option-", "menu-start-", "menu-whatever"... prefixes.</param>
        /// <returns>The original name without the prefixes. If the input string is null or empty, the original string is returned.</returns>
        public static string GetOriginalName(string nameWithPattern)
        {
            if (string.IsNullOrEmpty(nameWithPattern) || !nameWithPattern.Contains('-')) {
                return nameWithPattern;
            }

            // Regular expression to match and remove the first two words separated by dashes
            var regex = new Regex(@"^[^-]+-[^-]+-");
            return regex.Replace(nameWithPattern, string.Empty);
        }
    }
}
