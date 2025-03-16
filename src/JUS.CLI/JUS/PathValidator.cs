// Copyright (c) 2022 Priverop

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
using System.IO;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Auxiliar validation methods for files and directories.
    /// </summary>
    public static class PathValidator
    {
        /// <summary>
        /// Validates the path of a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <exception cref="ArgumentException">Thrown if the path is empty or null.</exception>
        public static void ValidateFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("The file path cannot be empty or null.");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file does not exist: {filePath}", filePath);
            }
        }

        /// <summary>
        /// Validates the path of a directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
        /// <exception cref="ArgumentException">Thrown if the path is empty or null.</exception>
        public static void ValidateDirectory(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException("The directory path cannot be empty or null.");
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory does not exist: {directoryPath}");
            }
        }
    }
}
