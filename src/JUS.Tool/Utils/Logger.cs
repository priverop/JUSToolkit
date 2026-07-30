// Copyright (c) 2026 Priverop

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

namespace JUS.Tool.Utils
{
    /// <summary>
    /// Helper methods for logging.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Calls the DisplayError function, for a length limit reached in a string sentence.
        /// </summary>
        /// <param name="maxLength">Maximum length of the sentence.</param>
        /// <param name="sentence">The problematic sentence.</param>
        public static void DisplayErrorMaxLength(int maxLength, string sentence)
        {
            DisplayError($"Limit of {maxLength} chars reached: {sentence}.");
        }

        /// <summary>
        /// Calls the DisplayError function, for a limit number of lines reached in a string.
        /// </summary>
        /// <param name="maxLines">Maximum number of lines.</param>
        /// <param name="sentence">The problematic sentence.</param>
        public static void DisplayErrorMaxLines(int maxLines, string sentence)
        {
            DisplayError($"Limit of {maxLines} lines reached: {sentence}.");
        }

        /// <summary>
        /// Displays the error in red, with an emoji.
        /// </summary>
        /// <param name="error">The error string.</param>
        public static void DisplayError(string error)
        {
            if (string.IsNullOrEmpty(error)) {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {error}");
            Console.ResetColor();
        }
    }
}
