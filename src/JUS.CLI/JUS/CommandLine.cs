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
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using JUSToolkit.Containers.Converters;
using Texim.Formats;
using Texim.Images;
using Texim.Sprites;
using Yarhl.FileSystem;

namespace JUSToolkit.CLI.JUS
{
    /// <summary>
    /// Command-line interface for Jump Ultimate Stars! game.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Create the CLI command for the game.
        /// </summary>
        /// <returns>The CLI command.</returns>
        public static Command CreateCommand()
        {
            return new Command("jus", "Jump Ultimate Stars! game") {
                CreateGraphicCommand(),
                CreateContainerCommand(),
            };
        }

        private static Command CreateGraphicCommand()
        {
            var exportDtx = new Command("export-dtx", "Export dtx") {
                new Option<string>("--container", "the input koma.aar container", ArgumentArity.ExactlyOne),
                new Option<string>("--koma", "the koma.bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--kshape", "the kshape.bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportDtx.Handler = CommandHandler.Create<string, string, string, string>(GraphicCommands.ExportDtx);

            var exportDsigAlmt = new Command("export-dsig-almt", "Export dsig+almt") {
                new Option<string>("--dsig", "the input file.dig", ArgumentArity.ExactlyOne),
                new Option<string>("--almt", "the input map.atm file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportDsigAlmt.Handler = CommandHandler.Create<string, string, string>(GraphicCommands.ExportDsigAlmt);

            return new Command("graphics", "Import/Export graphic files") {
                exportDtx,
                exportDsigAlmt,
            };
        }

        private static Command CreateContainerCommand()
        {
            var export = new Command("export-alar3", "Export alar3") {
                new Option<string>("--container", "the input alar3 container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            export.Handler = CommandHandler.Create<string, string>(ContainerCommands.ExportAlar3);

            return new Command("containers", "Unpack/Repack container files") {
                export,
            };
        }
    }
}
