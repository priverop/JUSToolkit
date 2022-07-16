using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using JUS.Tool.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Texts.Converters
{
    public class Binary2SuppChr :
        IConverter<BinaryFormat, SuppChr>,
        IConverter<SuppChr, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        public SuppChr Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var suppChr = new SuppChr();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / SuppChrEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                suppChr.Entries.Add(ReadEntry());
            }

            return suppChr;
        }

        public BinaryFormat Convert(SuppChr suppChr)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(SuppChrEntry.EntrySize * suppChr.Entries.Count);

            foreach (SuppChrEntry entry in suppChr.Entries) {
                JusText.WriteStringPointer(entry.chrName, writer, jit);
                foreach (string s in entry.Abilities) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Descriptions) {
                    JusText.WriteStringPointer(s, writer, jit);
                }
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        private SuppChrEntry ReadEntry()
        {
            var entry = new SuppChrEntry();

            entry.chrName = JusText.ReadIndirectString(reader);

            for (int i = 0; i < SuppChrEntry.NumAbilities * 2; i++) {
                entry.Abilities.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < SuppChrEntry.NumAbilities * 2; i++) {
                entry.Descriptions.Add(JusText.ReadIndirectString(reader));
            }

            return entry;
        }
    }
}
