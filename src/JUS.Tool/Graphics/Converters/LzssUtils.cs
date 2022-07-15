﻿// Copyright (c) 2022 Pablo Rivero

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
using System.Diagnostics;
using System.IO;
using System.Text;
using Yarhl.IO;

namespace JUSToolkit.Graphics.Converters
{
    /// <summary>
    /// Lzss methoods.
    /// </summary>
    public static class LzssUtils
    {
        private static string programExe = System.IO.Path.GetFullPath(@"..\..\..\..\..\") + @"\lib\NDS_Compressors_CUE\lzss.exe";
        private static string programUnix = System.IO.Path.GetFullPath(@"../../../../../") + "/lib/NDS_Compressors_CUE/lzss";

        /// <summary>
        /// Calls the external CUE's lzss library. It executes the unix binary.
        /// or the windows .exe.
        /// </summary>
        /// <param name="input">The input DataStream.</param>
        /// <param name="mode">Decompress with -d or compress the file with -evn.</param>
        /// <returns>The result DataStream.</returns>
        public static DataStream Lzss(DataStream input, string mode)
        {
            // We need a temporary file to execute the external program
            string tempFile = Path.GetRandomFileName();
            input.WriteTo(tempFile);

            string program = Environment.OSVersion.Platform == PlatformID.Win32NT ? programExe : programUnix;
            string arguments = mode + " " + tempFile;
            ExecuteExternalProcess(program, arguments);

            // As the external program overwrites the file, we need to create a new Stream
            DataStream fileStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);

            return fileStream;
        }

        /// <summary>
        /// Execute the external program.
        /// </summary>
        /// <param name="program">The program path.</param>
        /// <param name="arguments">The arguments for the program.</param>
        private static void ExecuteExternalProcess(string program, string arguments) {
            Process process = new Process();
            process.StartInfo.FileName = program;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
        }
    }
}
