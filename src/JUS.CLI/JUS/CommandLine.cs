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
                CreateContainerCommand(),
                CreateGraphicCommand(),
                CreateTextCommand(),
                CreateBatchCommand(),
                CreateRomCommand(),
            };
        }

        private static Command CreateGraphicCommand()
        {
            var exportKomas = new Command("export-komas", "Export komas") {
                new Option<string>("--container", "the input koma.aar container", ArgumentArity.ExactlyOne),
                new Option<string>("--koma", "the koma.bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--kshape", "the kshape.bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportKomas.Handler = CommandHandler.Create<string, string, string, string>(DtxCommands.ExportKomas);

            var importDtx4 = new Command("import-komas", "Import komas") {
                new Option<string>("--png", "the png to import", ArgumentArity.ExactlyOne),
                new Option<string>("--dtx", "the original file.dtx", ArgumentArity.ExactlyOne),
                new Option<string>("--koma", "the koma.bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--kshape", "the kshape.bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importDtx4.Handler = CommandHandler.Create<string, string, string, string, string>(DtxCommands.ImportKoma);

            var exportDtx4 = new Command("export-dtx4", "Export dtx") {
                new Option<string>("--dtx", "the input file.dtx", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportDtx4.Handler = CommandHandler.Create<string, string>(DtxCommands.ExportDtx4);

            var exportDtx3 = new Command("export-dtx3", "Export dtx") {
                new Option<string>("--dtx", "the input file.dtx", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportDtx3.Handler = CommandHandler.Create<string, string>(DtxCommands.ExportDtx3);

            var importDtx3 = new Command("import-dtx3", "Import dtx3") {
                new Option<string>("--input", "the new sprite to import", ArgumentArity.ExactlyOne),
                new Option<string>("--dtx", "the input file.dtx", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importDtx3.Handler = CommandHandler.Create<string, string, string>(DtxCommands.ImportDtx3);

            var exportDsigAlmt = new Command("export-dig", "Export dsig+almt") {
                new Option<string>("--dig", "the input file.dig", ArgumentArity.ExactlyOne),
                new Option<string>("--atm", "the input map.atm file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportDsigAlmt.Handler = CommandHandler.Create<string, string, string>(DigCommands.ExportDig);

            var importDig = new Command("import-dig", "Import dsig+almt") {
                new Option<string>("--input", "the png to import", ArgumentArity.ExactlyOne),
                new Option<bool>("--insertTransparent", "insert a transparent tile at the start of the .dig", ArgumentArity.ZeroOrOne),
                new Option<string>("--dig", "the original file.dig", ArgumentArity.ExactlyOne),
                new Option<string>("--atm", "the original file.atm", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importDig.Handler = CommandHandler.Create<string, bool, string, string, string>(DigCommands.ImportDig);

            var mergeDig = new Command("merge-dig", "Import Import dsig with multiple almt") {
                new Option<string>("--input", "the png to import", ArgumentArity.OneOrMore),
                new Option<bool>("--insertTransparent", "insert a transparent tile at the start of the .dig", ArgumentArity.ZeroOrOne),
                new Option<string>("--dig", "the original file.dig", ArgumentArity.ExactlyOne),
                new Option<string>("--atm", "the original file.atm", ArgumentArity.OneOrMore),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            mergeDig.Handler = CommandHandler.Create<string[], bool, string, string[], string>(DigCommands.MergeDig);

            return new Command("graphics", "Import/Export graphic files") {
                exportKomas,
                exportDtx3,
                exportDtx4,
                importDtx3,
                importDtx4,
                exportDsigAlmt,
                importDig,
                mergeDig,
            };
        }

        private static Command CreateContainerCommand()
        {
            var export = new Command("export", "Export alar container, any version") {
                new Option<string>("--container", "the input alar container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            export.Handler = CommandHandler.Create<string, string>(ContainerCommands.Export);

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

            var import = new Command("import", "import alar") {
                new Option<string>("--container", "the input alar container", ArgumentArity.ExactlyOne),
                new Option<string>("--input", "the input directory to insert", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            import.Handler = CommandHandler.Create<string, string, string>(ContainerCommands.Import);

            var importAlar2 = new Command("import-alar2", "import alar2") {
                new Option<string>("--container", "the input alar2 container", ArgumentArity.ExactlyOne),
                new Option<string>("--input", "the input directory to insert", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importAlar2.Handler = CommandHandler.Create<string, string, string>(ContainerCommands.ImportAlar2);

            var importAlar3 = new Command("import-alar3", "import alar3") {
                new Option<string>("--container", "the input alar3 container", ArgumentArity.ExactlyOne),
                new Option<string>("--input", "the input directory to insert", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importAlar3.Handler = CommandHandler.Create<string, string, string>(ContainerCommands.ImportAlar3);

            return new Command("containers", "Unpack/Repack container files") {
                export,
                exportAlar3,
                exportAlar2,
                import,
                importAlar2,
                importAlar3,
            };
        }

        private static Command CreateBatchCommand()
        {
            var importPng2Alar3 = new Command("import-png-alar3", "Batch import PNG to alar3") {
                new Option<string>("--container", "the original alar3 container", ArgumentArity.ExactlyOne),
                new Option<string>("--input", "the input directory to insert", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importPng2Alar3.Handler = CommandHandler.Create<string, string, string>(BatchCommands.ImportPng2Alar3);

            var exportAlarDig2Png = new Command("export-alar-dig-png", "Batch export PNGs from alar digs files") {
                new Option<string>("--container", "the alar container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportAlarDig2Png.Handler = CommandHandler.Create<string, string>(BatchCommands.ExportAlarDig2Png);

            var exportAlarDtx2Png = new Command("export-alar-dtx-png", "Batch export PNGs from alar dtxs files") {
                new Option<string>("--container", "the alar container", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportAlarDtx2Png.Handler = CommandHandler.Create<string, string>(BatchCommands.ExportAlarDtx2Png);

            return new Command("batch", "Batch import/export PNG to/from Alar") {
                importPng2Alar3,
                exportAlarDig2Png,
                exportAlarDtx2Png,
            };
        }

        private static Command CreateRomCommand()
        {
            var import = new Command("import", "Import files to the Rom") {
                new Option<string>("--game", "the path of the rom", ArgumentArity.ExactlyOne),
                new Option<string>("--input", "the input directory to insert", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            import.Handler = CommandHandler.Create<string, string, string>(RomCommands.Import);

            var importFont = new Command("importFont", "Import the translated font to the Rom") {
                new Option<string>("--game", "the path of the rom", ArgumentArity.ExactlyOne),
                new Option<string>("--font", "the font to insert", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importFont.Handler = CommandHandler.Create<string, string, string>(RomCommands.ImportFont);

            return new Command("game", "Import files to the Game") {
                import,
                importFont,
            };
        }

        private static Command CreateTextCommand()
        {
            var export = new Command("export", "Export .bin file to a .po file") {
                new Option<string>("--bin", "the input .bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            export.Handler = CommandHandler.Create<string, string>(TextExportCommands.Export);

            var exportJQuiz = new Command("exportJQuiz", "Export JQuiz .bin file to multiple .po files") {
                new Option<string>("--bin", "the input jquiz.bin file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            exportJQuiz.Handler = CommandHandler.Create<string, string>(TextExportCommands.JQuizExport);

            var deckExport = new Command("deckExport", "Export .bin files from a folder, to a single .po file") {
                new Option<string>("--directory", "the directory with the .bin files", ArgumentArity.ExactlyOne),
                new Option<bool>("--pdeck", "true for pdeck files", ArgumentArity.ZeroOrOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            deckExport.Handler = CommandHandler.Create<string, bool, string>(TextExportCommands.DeckExport);

            var batchExport = new Command("batchExport", "Export .bin files from a folder, to .po files") {
                new Option<string>("--directory", "the input directory with the .bin files", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            batchExport.Handler = CommandHandler.Create<string, string>(TextExportCommands.BatchExport);

            var import = new Command("import", "Import a .po file into a .bin") {
                new Option<string>("--po", "the input .po file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            import.Handler = CommandHandler.Create<string, string>(TextImportCommands.Import);

            var deckImport = new Command("deckImport", "Import a single po file, to multiple .bin files") {
                new Option<string>("--po", "the po file", ArgumentArity.ExactlyOne),
                new Option<bool>("--pdeck", "true for pdeck files", ArgumentArity.ZeroOrOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            deckImport.Handler = CommandHandler.Create<string, bool, string>(TextImportCommands.DeckImport);

            var batchImport = new Command("batchImport", "Import .bin files from a folder, to a .po file") {
                new Option<string>("--directory", "the input directory with the .bin files", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            batchImport.Handler = CommandHandler.Create<string, string>(TextImportCommands.BatchImport);

            var importJQuiz = new Command("importjquiz", "Import the jquiz Po folder into a .bin") {
                new Option<string>("--container", "the container of the .po files", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder", ArgumentArity.ExactlyOne),
            };
            importJQuiz.Handler = CommandHandler.Create<string, string>(TextImportCommands.ImportJQuiz);

            return new Command("texts", "Export or import bin files to Po") {
                export,
                deckExport,
                exportJQuiz,
                batchExport,
                import,
                deckImport,
                batchImport,
                importJQuiz,
            };
        }
    }
}
