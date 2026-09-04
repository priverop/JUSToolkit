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
using JUS.CLI.JUS.Graphics;
using System.CommandLine;

namespace JUS.CLI.JUS
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
                FontCommands.CreateCommands(),
            };
        }

        private static Command CreateGraphicCommand()
        {
            var exportKomasContainer = new Option<string>("--container") { Description = "the input koma.aar container" };
            var exportKomasKoma = new Option<string>("--koma") { Description = "the koma.bin file" };
            var exportKomasKshape = new Option<string>("--kshape") { Description = "the kshape.bin file" };
            var exportKomasOutput = new Option<string>("--output") { Description = "the output folder" };
            var exportKomas = new Command("export-komas", "Export komas") {
                exportKomasContainer,
                exportKomasKoma,
                exportKomasKshape,
                exportKomasOutput,
            };
            exportKomas.SetAction(parseResult => {
                DtxCommands.ExportKomas(
                    parseResult.GetValue(exportKomasContainer)!,
                    parseResult.GetValue(exportKomasKoma)!,
                    parseResult.GetValue(exportKomasKshape)!,
                    parseResult.GetValue(exportKomasOutput)!);
            });

            var importKomasPng = new Option<string>("--png") { Description = "the png to import" };
            var importKomasDtx = new Option<string>("--dtx") { Description = "the original file.dtx" };
            var importKomasKoma = new Option<string>("--koma") { Description = "the koma.bin file" };
            var importKomasKshape = new Option<string>("--kshape") { Description = "the kshape.bin file" };
            var importKomasOutput = new Option<string>("--output") { Description = "the output folder" };
            var importDtx4 = new Command("import-komas", "Import komas") {
                importKomasPng,
                importKomasDtx,
                importKomasKoma,
                importKomasKshape,
                importKomasOutput,
            };
            importDtx4.SetAction(parseResult => {
                DtxCommands.ImportKoma(
                    parseResult.GetValue(importKomasPng)!,
                    parseResult.GetValue(importKomasDtx)!,
                    parseResult.GetValue(importKomasKoma)!,
                    parseResult.GetValue(importKomasKshape)!,
                    parseResult.GetValue(importKomasOutput)!);
            });

            var exportDtx4Dtx = new Option<string>("--dtx") { Description = "the input file.dtx" };
            var exportDtx4Koma = new Option<string>("--koma") { Description = "the koma.bin file" };
            var exportDtx4Kshape = new Option<string>("--kshape") { Description = "the kshape.bin file" };
            var exportDtx4Output = new Option<string>("--output") { Description = "the output folder" };
            var exportDtx4 = new Command("export-dtx4", "Export dtx") {
                exportDtx4Dtx,
                exportDtx4Koma,
                exportDtx4Kshape,
                exportDtx4Output,
            };
            exportDtx4.SetAction(parseResult => {
                DtxCommands.ExportDtx4(
                    parseResult.GetValue(exportDtx4Dtx)!,
                    parseResult.GetValue(exportDtx4Koma)!,
                    parseResult.GetValue(exportDtx4Kshape)!,
                    parseResult.GetValue(exportDtx4Output)!);
            });

            var exportDtx3Dtx = new Option<string>("--dtx") { Description = "the input file.dtx" };
            var exportDtx3Output = new Option<string>("--output") { Description = "the output folder" };
            var exportDtx3 = new Command("export-dtx3", "Export dtx") {
                exportDtx3Dtx,
                exportDtx3Output,
            };
            exportDtx3.SetAction(parseResult => {
                DtxCommands.ExportDtx3(
                    parseResult.GetValue(exportDtx3Dtx)!,
                    parseResult.GetValue(exportDtx3Output)!);
            });

            var exportDtx3TxDtx = new Option<string>("--dtx") { Description = "the input file.dtx" };
            var exportDtx3TxOutput = new Option<string>("--output") { Description = "the output folder" };
            var exportDtx3TxImage = new Command("export-dtx3tx-image", "Export dtx3 image") {
                exportDtx3TxDtx,
                exportDtx3TxOutput,
            };
            exportDtx3TxImage.SetAction(parseResult => {
                DtxCommands.ExportDtx3TxImage(
                    parseResult.GetValue(exportDtx3TxDtx)!,
                    parseResult.GetValue(exportDtx3TxOutput)!);
            });

            var importDtx3Input = new Option<string>("--input") { Description = "the folder with the .pngs to import" };
            var importDtx3Dtx = new Option<string>("--dtx") { Description = "the input file.dtx" };
            var importDtx3Output = new Option<string>("--output") { Description = "the output folder" };
            var importDtx3 = new Command("import-dtx3", "Import dtx3") {
                importDtx3Input,
                importDtx3Dtx,
                importDtx3Output,
            };
            importDtx3.SetAction(parseResult => {
                DtxCommands.ImportDtx3(
                    parseResult.GetValue(importDtx3Input)!,
                    parseResult.GetValue(importDtx3Dtx)!,
                    parseResult.GetValue(importDtx3Output)!);
            });

            var importDtx3TxInput = new Option<string>("--input") { Description = "the folder with the .pngs to import" };
            var importDtx3TxDtx = new Option<string>("--dtx") { Description = "the input file.dtx" };
            var importDtx3TxYaml = new Option<string>("--yaml") { Description = "segments metadata" };
            var importDtx3TxOutput = new Option<string>("--output") { Description = "the output folder" };
            var importDtx3Tx = new Command("import-dtx3tx", "Import dtx3") {
                importDtx3TxInput,
                importDtx3TxDtx,
                importDtx3TxYaml,
                importDtx3TxOutput,
            };
            importDtx3Tx.SetAction(parseResult => {
                DtxCommands.ImportDtx3Tx(
                    parseResult.GetValue(importDtx3TxInput)!,
                    parseResult.GetValue(importDtx3TxDtx)!,
                    parseResult.GetValue(importDtx3TxYaml)!,
                    parseResult.GetValue(importDtx3TxOutput)!);
            });

            var exportDigDig = new Option<string>("--dig") { Description = "the input file.dig" };
            var exportDigAtm = new Option<string>("--atm") { Description = "the input map.atm file" };
            var exportDigOutput = new Option<string>("--output") { Description = "the output folder" };
            var exportDsigAltm = new Command("export-dig", "Export dsig+altm") {
                exportDigDig,
                exportDigAtm,
                exportDigOutput,
            };
            exportDsigAltm.SetAction(parseResult => {
                DigCommands.ExportDig(
                    parseResult.GetValue(exportDigDig)!,
                    parseResult.GetValue(exportDigAtm)!,
                    parseResult.GetValue(exportDigOutput)!);
            });

            var importDigInput = new Option<string>("--input") { Description = "the png to import" };
            var importDigInsertTransparent = new Option<bool>("--insertTransparent") { Description = "insert a transparent tile at the start of the .dig" };
            var importDigDig = new Option<string>("--dig") { Description = "the original file.dig" };
            var importDigAtm = new Option<string>("--atm") { Description = "the original file.atm" };
            var importDigOutput = new Option<string>("--output") { Description = "the output folder" };
            var importDig = new Command("import-dig", "Import dsig+altm") {
                importDigInput,
                importDigInsertTransparent,
                importDigDig,
                importDigAtm,
                importDigOutput,
            };
            importDig.SetAction(parseResult => {
                DigCommands.ImportDig(
                    parseResult.GetValue(importDigInput)!,
                    parseResult.GetValue(importDigInsertTransparent),
                    parseResult.GetValue(importDigDig)!,
                    parseResult.GetValue(importDigAtm)!,
                    parseResult.GetValue(importDigOutput)!);
            });

            var mergeDigInput = new Option<string[]>("--input") { Description = "the png to import", Arity = ArgumentArity.OneOrMore };
            var mergeDigInsertTransparent = new Option<bool>("--insertTransparent") { Description = "insert a transparent tile at the start of the .dig" };
            var mergeDigDig = new Option<string>("--dig") { Description = "the original file.dig" };
            var mergeDigAtm = new Option<string[]>("--atm") { Description = "the original file.atm", Arity = ArgumentArity.OneOrMore };
            var mergeDigOutput = new Option<string>("--output") { Description = "the output folder" };
            var mergeDig = new Command("merge-dig", "Import Import dsig with multiple altm") {
                mergeDigInput,
                mergeDigInsertTransparent,
                mergeDigDig,
                mergeDigAtm,
                mergeDigOutput,
            };
            mergeDig.SetAction(parseResult => {
                DigCommands.MergeDig(
                    parseResult.GetValue(mergeDigInput)!,
                    parseResult.GetValue(mergeDigInsertTransparent),
                    parseResult.GetValue(mergeDigDig)!,
                    parseResult.GetValue(mergeDigAtm)!,
                    parseResult.GetValue(mergeDigOutput)!);
            });

            var exportYamlDtx3Dtx = new Option<string>("--dtx") { Description = "the input file.dtx" };
            var exportYamlDtx3Output = new Option<string>("--output") { Description = "the output folder" };
            var exportYamlDtx3 = new Command("export-yaml-dtx3", "Export the sprite metadata") {
                exportYamlDtx3Dtx,
                exportYamlDtx3Output,
            };
            exportYamlDtx3.SetAction(parseResult => {
                DtxCommands.ExportYamlDtx3(
                    parseResult.GetValue(exportYamlDtx3Dtx)!,
                    parseResult.GetValue(exportYamlDtx3Output)!);
            });

            return new Command("graphics", "Import/Export graphic files") {
                exportKomas,
                exportDtx3,
                exportDtx3TxImage,
                exportYamlDtx3,
                exportDtx4,
                importDtx3,
                importDtx3Tx,
                importDtx4,
                exportDsigAltm,
                importDig,
                mergeDig,
            };
        }

        private static Command CreateContainerCommand()
        {
            var exportContainer = new Option<string>("--container") { Description = "the input alar container" };
            var exportOutput = new Option<string>("--output") { Description = "the output folder" };
            var export = new Command("export", "Export alar container, any version") {
                exportContainer,
                exportOutput,
            };
            export.SetAction(parseResult => {
                ContainerCommands.Export(
                    parseResult.GetValue(exportContainer)!,
                    parseResult.GetValue(exportOutput)!);
            });

            var importContainer = new Option<string>("--container") { Description = "the input alar container" };
            var importInput = new Option<string>("--input") { Description = "the input directory to insert" };
            var importOutput = new Option<string>("--output") { Description = "the output folder" };
            var import = new Command("import", "import alar") {
                importContainer,
                importInput,
                importOutput,
            };
            import.SetAction(parseResult => {
                ContainerCommands.Import(
                    parseResult.GetValue(importContainer)!,
                    parseResult.GetValue(importInput)!,
                    parseResult.GetValue(importOutput)!);
            });

            return new Command("containers", "Unpack/Repack container files") {
                export,
                import,
            };
        }

        private static Command CreateBatchCommand()
        {
            var importPng2Alar3Container = new Option<string>("--container") { Description = "the original alar3 container" };
            var importPng2Alar3Input = new Option<string>("--input") { Description = "the input directory to insert" };
            var importPng2Alar3Output = new Option<string>("--output") { Description = "the output folder" };
            var importPng2Alar3 = new Command("import-png-alar3", "Batch import PNG to alar3") {
                importPng2Alar3Container,
                importPng2Alar3Input,
                importPng2Alar3Output,
            };
            importPng2Alar3.SetAction(parseResult => {
                BatchCommands.ImportPng2Alar3(
                    parseResult.GetValue(importPng2Alar3Container)!,
                    parseResult.GetValue(importPng2Alar3Input)!,
                    parseResult.GetValue(importPng2Alar3Output)!);
            });

            var exportAlarDig2PngContainer = new Option<string>("--container") { Description = "the alar container" };
            var exportAlarDig2PngOutput = new Option<string>("--output") { Description = "the output folder" };
            var exportAlarDig2Png = new Command("export-alar-dig-png", "Batch export PNGs from alar digs files") {
                exportAlarDig2PngContainer,
                exportAlarDig2PngOutput,
            };
            exportAlarDig2Png.SetAction(parseResult => {
                BatchCommands.ExportAlarDig2Png(
                    parseResult.GetValue(exportAlarDig2PngContainer)!,
                    parseResult.GetValue(exportAlarDig2PngOutput)!);
            });

            var exportAlarDtx2PngContainer = new Option<string>("--container") { Description = "the alar container" };
            var exportAlarDtx2PngOutput = new Option<string>("--output") { Description = "the output folder" };
            var exportAlarDtx2PngGetImage = new Option<bool>("--getImage") { Description = "only export the image" };
            var exportAlarDtx2Png = new Command("export-alar-dtx-png", "Batch export PNGs from alar dtxs files") {
                exportAlarDtx2PngContainer,
                exportAlarDtx2PngOutput,
                exportAlarDtx2PngGetImage,
            };
            exportAlarDtx2Png.SetAction(parseResult => {
                BatchCommands.ExportAlarDtx2Png(
                    parseResult.GetValue(exportAlarDtx2PngContainer)!,
                    parseResult.GetValue(exportAlarDtx2PngOutput)!,
                    parseResult.GetValue(exportAlarDtx2PngGetImage));
            });

            var uncompressEverythingInput = new Option<string>("--input") { Description = "the input directory with all the files" };
            var uncompressEverything = new Command("uncompress-everything", "Batch uncompress every file") {
                uncompressEverythingInput,
            };
            uncompressEverything.SetAction(parseResult => {
                BatchCommands.UncompressFiles(
                    parseResult.GetValue(uncompressEverythingInput)!);
            });

            return new Command("batch", "Batch import/export PNG to/from Alar") {
                importPng2Alar3,
                exportAlarDig2Png,
                exportAlarDtx2Png,
                uncompressEverything,
            };
        }

        private static Command CreateRomCommand()
        {
            var importGame = new Option<string>("--game") { Description = "the path of the rom" };
            var importInput = new Option<string>("--input") { Description = "the input directory to insert" };
            var importOutput = new Option<string>("--output") { Description = "the output folder" };
            var import = new Command("import", "Import files to the Rom") {
                importGame,
                importInput,
                importOutput,
            };
            import.SetAction(parseResult => {
                RomCommands.Import(
                    parseResult.GetValue(importGame)!,
                    parseResult.GetValue(importInput)!,
                    parseResult.GetValue(importOutput)!);
            });

            var importFontGame = new Option<string>("--game") { Description = "the path of the rom" };
            var importFontFont = new Option<string>("--font") { Description = "the font to insert" };
            var importFontOutput = new Option<string>("--output") { Description = "the output folder" };
            var importFont = new Command("import-font", "Import the translated font to the Rom") {
                importFontGame,
                importFontFont,
                importFontOutput,
            };
            importFont.SetAction(parseResult => {
                RomCommands.ImportFont(
                    parseResult.GetValue(importFontGame)!,
                    parseResult.GetValue(importFontFont)!,
                    parseResult.GetValue(importFontOutput)!);
            });

            return new Command("game", "Import files to the Game") {
                import,
                importFont,
            };
        }

        private static Command CreateTextCommand()
        {
            var exportBin = new Option<string>("--bin") { Description = "the input .bin file" };
            var exportOutput = new Option<string>("--output") { Description = "the output folder" };
            var export = new Command("export", "Export .bin file to a .po file") {
                exportBin,
                exportOutput,
            };
            export.SetAction(parseResult => {
                TextExportCommands.Export(
                    parseResult.GetValue(exportBin)!,
                    parseResult.GetValue(exportOutput)!);
            });

            var exportJQuizBin = new Option<string>("--bin") { Description = "the input jquiz.bin file" };
            var exportJQuizOutput = new Option<string>("--output") { Description = "the output folder" };
            var exportJQuiz = new Command("export-jquiz", "Export JQuiz .bin file to multiple .po files") {
                exportJQuizBin,
                exportJQuizOutput,
            };
            exportJQuiz.SetAction(parseResult => {
                TextExportCommands.JQuizExport(
                    parseResult.GetValue(exportJQuizBin)!,
                    parseResult.GetValue(exportJQuizOutput)!);
            });

            var deckExportDirectory = new Option<string>("--directory") { Description = "the directory with the .bin files" };
            var deckExportPdeck = new Option<bool>("--pdeck") { Description = "true for pdeck files" };
            var deckExportOutput = new Option<string>("--output") { Description = "the output folder" };
            var deckExport = new Command("export-deck", "Export deck/pdeck .bin files from a folder, to a single .po file") {
                deckExportDirectory,
                deckExportPdeck,
                deckExportOutput,
            };
            deckExport.SetAction(parseResult => {
                TextExportCommands.DeckExport(
                    parseResult.GetValue(deckExportDirectory)!,
                    parseResult.GetValue(deckExportPdeck),
                    parseResult.GetValue(deckExportOutput)!);
            });

            var batchExportDirectory = new Option<string>("--directory") { Description = "the input directory with the .bin files" };
            var batchExportOutput = new Option<string>("--output") { Description = "the output folder" };
            var batchExport = new Command("batch-export", "Batch export .bin files from a folder, to .po files") {
                batchExportDirectory,
                batchExportOutput,
            };
            batchExport.SetAction(parseResult => {
                TextExportCommands.BatchExport(
                    parseResult.GetValue(batchExportDirectory)!,
                    parseResult.GetValue(batchExportOutput)!);
            });

            var importPo = new Option<string>("--po") { Description = "the input .po file" };
            var importOutput = new Option<string>("--output") { Description = "the output folder" };
            var import = new Command("import", "Import a .po file into a .bin") {
                importPo,
                importOutput,
            };
            import.SetAction(parseResult => {
                TextImportCommands.Import(
                    parseResult.GetValue(importPo)!,
                    parseResult.GetValue(importOutput)!);
            });

            var deckImportPo = new Option<string>("--po") { Description = "the po file" };
            var deckImportPdeck = new Option<bool>("--pdeck") { Description = "true for pdeck files" };
            var deckImportOutput = new Option<string>("--output") { Description = "the output folder" };
            var deckImport = new Command("import-deck", "Import a single po file, to multiple .bin files") {
                deckImportPo,
                deckImportPdeck,
                deckImportOutput,
            };
            deckImport.SetAction(parseResult => {
                TextImportCommands.DeckImport(
                    parseResult.GetValue(deckImportPo)!,
                    parseResult.GetValue(deckImportPdeck),
                    parseResult.GetValue(deckImportOutput)!);
            });

            var batchImportDirectory = new Option<string>("--directory") { Description = "the input directory with the .bin files" };
            var batchImportOutput = new Option<string>("--output") { Description = "the output folder" };
            var batchImport = new Command("batch-import", "Import .bin files from a folder, to a .po file") {
                batchImportDirectory,
                batchImportOutput,
            };
            batchImport.SetAction(parseResult => {
                TextImportCommands.BatchImport(
                    parseResult.GetValue(batchImportDirectory)!,
                    parseResult.GetValue(batchImportOutput)!);
            });

            var importJQuizContainer = new Option<string>("--container") { Description = "the container of the .po files" };
            var importJQuizOutput = new Option<string>("--output") { Description = "the output folder" };
            var importJQuiz = new Command("import-jquiz", "Import the jquiz Po folder into a .bin") {
                importJQuizContainer,
                importJQuizOutput,
            };
            importJQuiz.SetAction(parseResult => {
                TextImportCommands.ImportJQuiz(
                    parseResult.GetValue(importJQuizContainer)!,
                    parseResult.GetValue(importJQuizOutput)!);
            });

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
