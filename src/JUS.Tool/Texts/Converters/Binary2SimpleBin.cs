using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUS.Tool.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Texts.Converters
{
    public class Binary2SimpleBin :
        IConverter<BinaryFormat, SimpleBin>,
        IConverter<SimpleBin, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        public SimpleBin Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var simple = new SimpleBin();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / SimpleBin.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                simple.TextEntries.Add(JusText.ReadIndirectString(reader));
            }

            return simple;
        }

        public BinaryFormat Convert(SimpleBin simpleBin)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(SimpleBin.EntrySize * simpleBin.TextEntries.Count);

            foreach (string entry in simpleBin.TextEntries) {
                JusText.WriteStringPointer(entry, writer, jit);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }
    }
}
