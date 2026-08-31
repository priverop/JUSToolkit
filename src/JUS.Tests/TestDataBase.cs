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
using NUnit.Framework;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.IO;

namespace JUS.Tests
{
    /// <summary>
    /// Managing test resources.
    /// </summary>
    public static class TestDataBase
    {
        public static string RootFromOutputPath {
            get {
                string? envVar = Environment.GetEnvironmentVariable("JUS_PATH");
                if (!string.IsNullOrEmpty(envVar)) {
                    return envVar;
                }

                string programDir = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(
                    programDir,
                    "..", // output folder (framework) -> debug/release (configuration)
                    "..", // -> bin
                    "..", // -> project
                    "Resources");
                return Path.GetFullPath(path);
            }
        }

        public static string SoftwareNitroRomPath => Path.Combine(RootFromOutputPath, "JUS_AJUJ01_00.nds");

        public static string RootTestFailedPath =>
            Path.Combine(Path.GetDirectoryName(SoftwareNitroRomPath)!, "failed");


        public static NitroRom ReadSoftware()
        {
            string path = SoftwareNitroRomPath;
            IgnoreIfFileDoesNotExist(path);

            using var binary = new BinaryFormat(path, FileOpenMode.Read);
            return new Binary2NitroRom().Convert(binary);
        }

        public static void IgnoreIfFileDoesNotExist(string file)
        {
            if (!File.Exists(file)) {
                string msg = $"[{TestContext.CurrentContext.Test.ClassName}] Missing resource file: {file}";
                TestContext.Progress.WriteLine(msg);
                Assert.Ignore(msg);
            }
        }

        public static IEnumerable<string> ReadTestListFile(string filePath)
        {
            return !File.Exists(filePath)
                ? Array.Empty<string>()
                : File.ReadAllLines(filePath)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith('#'));
        }

        public static void WriteFailedData(Stream? stream, string name)
        {
            string path = Path.Combine(RootTestFailedPath, name);
            stream?.WriteTo(path);
        }

    }
}
