// Copyright (c) 2020 Benito Palacios Sánchez
//
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
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.io.FileWriter;
import java.util.List;
import java.util.Optional;

import ghidra.app.script.GhidraScript;
import ghidra.program.database.mem.FileBytes;
import ghidra.program.flatapi.FlatProgramAPI;
import ghidra.program.model.data.AbstractStringDataType;
import ghidra.program.model.data.DataType;
import ghidra.program.model.listing.Data;
import ghidra.program.model.mem.MemoryBlock;
import ghidra.program.model.mem.MemoryBlockSourceInfo;
import ghidra.program.model.symbol.Reference;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

public class ExportDefinedStrings extends GhidraScript {
    private Logger logger;

    public ExportDefinedStrings()
    {
        logger = LogManager.getLogger(ExportDefinedStrings.class);
    }

    @Override
    protected void run() throws Exception {
        String[] args = getScriptArgs();
        if (args.length != 2) {
            logger.error("USAGE: ExportDefinedString.java <output_file> <addr_padding>");
            return;
        }

        String outputPath = args[0];
        int addrPadding = Integer.parseInt(args[1]);
        logger.info("Exporting defined strings with padding " + addrPadding + " into " + outputPath);
        FileWriter outputWriter = new FileWriter(outputPath, StandardCharsets.UTF_8);

        FlatProgramAPI api = new FlatProgramAPI(currentProgram);
        exportMemoryBlocks(api.getMemoryBlocks(), outputWriter);
        exportStringDefinitions(api, addrPadding, outputWriter);

        outputWriter.close();
    }

    private void exportMemoryBlocks(MemoryBlock[] blocks, FileWriter outputWriter)
        throws IOException
    {
        outputWriter.write("memoryBlocks:\n");
        for (MemoryBlock block : blocks) {
            List<MemoryBlockSourceInfo> sources = block.getSourceInfos();
            if (sources.size() != 1) {
                logger.warn("WARN: Block without or with multiple sources. Ignoring: " + block.getName());
                continue;
            }

            long ramAddr = block.getStart().getOffset();
            outputWriter.write("  - name: '" + block.getName() + "'\n");
            outputWriter.write("    address: 0x" + Long.toHexString(ramAddr) + "\n");
            outputWriter.write("    size: " + block.getSize() + "\n");

            MemoryBlockSourceInfo source = sources.get(0);
            Optional<FileBytes> sourceFile = source.getFileBytes();
            if (!sourceFile.isPresent()) {
                logger.info("Block without associated file: " + block.getName());
            } else {
                String fileName = sourceFile.get().getFilename();
                long fileOffset = sourceFile.get().getFileOffset();
                outputWriter.write("    fileName: '" + fileName + "'\n");
                outputWriter.write("    fileOffset: 0x" + Long.toHexString(fileOffset) + "\n");
            }

            outputWriter.write("\n");
        }
    }

    private void exportStringDefinitions(FlatProgramAPI api, int addrPadding, FileWriter outputWriter)
        throws IOException
    {
        outputWriter.write("strings:\n");
        Data data = api.getFirstData();
        while (data != null) {
            DataType type = data.getDataType();
            if (type instanceof AbstractStringDataType) {
                AbstractStringDataType stringType = (AbstractStringDataType)type;
                exportString(data, stringType, addrPadding, outputWriter);
            }

            data = api.getDataAfter(data);
        }
    }

    private void exportString(Data data, AbstractStringDataType type, int addrPadding, FileWriter outputWriter)
        throws IOException
    {
        String address = data.getAddressString(true, true); // showBlockName, pad
        long length = data.getLength();
        if (addrPadding > 0) {
            length = Pad(length, addrPadding);
        }

        String value = data.getValue().toString();
        String charset = type.getCharsetName(data);

        // Replace new lines with the comment sign for multi-line strings be multi-line comments
        value = value.replace("\n", "\n  # ")
            .replace("\r", "")
            .replace("\t", "\\t"); // tabs cases issues in YAML

        outputWriter.write("  # " + value + "\n");
        outputWriter.write("  - address: '" + address + "'\n");
        outputWriter.write("    size: " + length + "\n");
        outputWriter.write("    encoding: '" + charset + "'\n");
        outputWriter.write("    pointers:\n");

        for (Reference ref : data.getReferenceIteratorTo()) {
            outputWriter.write("      - '" + ref.getFromAddress().toString(true, true) + "'\n");
        }

        outputWriter.write("\n");
    }

    private static long Pad(long address, long padding)
    {
        return address + ((address % padding == 0) ? 0 : padding - (address % padding));
    }
}
