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
using System.CommandLine;
using System.CommandLine.Invocation;

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

            var exportDtx3 = new Command("export-dtx3", "Export dtx") {
                new Option<string>("--dtx", "the input file.dtx", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportDtx3.Handler = CommandHandler.Create<string, string>(GraphicCommands.ExportDtx3);

            var exportDsigAlmt = new Command("export-dig", "Export dsig+almt") {
                new Option<string>("--dig", "the input file.dig", ArgumentArity.ExactlyOne),
                new Option<string>("--atm", "the input map.atm file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportDsigAlmt.Handler = CommandHandler.Create<string, string, string>(GraphicCommands.ExportDig);

            var importDig = new Command("import-dig", "Import dsig+almt") {
                new Option<string>("--input", "the png to import", ArgumentArity.ExactlyOne),
                new Option<string>("--dig", "the original file.dig", ArgumentArity.ExactlyOne),
                new Option<string>("--atm", "the original file.atm", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importDig.Handler = CommandHandler.Create<string, string, string, string>(GraphicCommands.ImportDig);

            return new Command("graphics", "Import/Export graphic files") {
                exportDtx,
                exportDtx3,
                exportDsigAlmt,
                importDig,
            };
        }

        private static Command CreateContainerCommand()
        {
            var exportAlar3 = new Command("export-alar3", "Export alar3") {
                new Option<string>("--container", "the input alar3 container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportAlar3.Handler = CommandHandler.Create<string, string>(ContainerCommands.ExportAlar3);

            var exportAlar2 = new Command("export-alar2", "Export alar2") {
                new Option<string>("--container", "the input alar2 container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportAlar2.Handler = CommandHandler.Create<string, string>(ContainerCommands.ExportAlar2);

            var importAlar2 = new Command("import-alar2", "import alar2") {
                new Option<string>("--container", "the input alar2 container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importAlar2.Handler = CommandHandler.Create<string, string>(ContainerCommands.ImportAlar2);

            var importAlar3 = new Command("import-alar3", "import alar3") {
                new Option<string>("--container", "the input alar3 container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importAlar3.Handler = CommandHandler.Create<string, string>(ContainerCommands.ImportAlar3);

            return new Command("containers", "Unpack/Repack container files") {
                exportAlar3,
                exportAlar2,
                importAlar2,
                importAlar3,
            };
        }
    }
}
