using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    public class Binary2Demo :
        IConverter<BinaryFormat, Demo>,
        IConverter<Demo, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        public Demo Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }
            var demo = new Demo();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            demo.Count = reader.ReadInt32();
            for (int i = 0; i < demo.Count; i++) {
                demo.Entries.Add(ReadEntry());
            }

            return demo;
        }

        public BinaryFormat Convert(Demo demo)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText((DemoEntry.EntrySize * demo.Count) + 0x04);

            writer.Write(demo.Count);

            foreach (DemoEntry entry in demo.Entries) {
                JusText.WriteStringPointer(entry.Title, writer, jit);
                JusText.WriteStringPointer(entry.Desc1, writer, jit);
                JusText.WriteStringPointer(entry.Desc2, writer, jit);
                JusText.WriteStringPointer(entry.Desc3, writer, jit);
                writer.Write(entry.Id);
                writer.Write(entry.Icon);
                writer.WritePadding(0x00, 0x04);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="DemoEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="DemoEntry"/>.</returns>
        private DemoEntry ReadEntry()
        {
            var entry = new DemoEntry();

            entry.Title = JusText.ReadIndirectString(reader);
            entry.Desc1 = JusText.ReadIndirectString(reader);
            entry.Desc2 = JusText.ReadIndirectString(reader);
            entry.Desc3 = JusText.ReadIndirectString(reader);
            entry.Id = reader.ReadByte();
            entry.Icon = reader.ReadByte();

            reader.SkipPadding(0x04);

            return entry;
        }
    }
}
