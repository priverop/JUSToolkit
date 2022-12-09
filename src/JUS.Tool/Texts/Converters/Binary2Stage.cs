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
    public class Binary2Stage :
        IConverter<BinaryFormat, Stage>,
        IConverter<Stage, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        public Stage Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var stage = new Stage();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            int count = reader.ReadInt32() / StageEntry.EntrySize;
            reader.Stream.Position = 0x00;

            for (int i = 0; i < count; i++) {
                stage.Entries.Add(ReadEntry());
            }

            return stage;
        }

        public BinaryFormat Convert(Stage stage)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(StageEntry.EntrySize * stage.Entries.Count);

            foreach (StageEntry entry in stage.Entries) {
                JusText.WriteStringPointer(entry.Name, writer, jit);
                writer.Write(entry.Unk1);
                writer.Write(entry.Unk2);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="StageEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="StageEntry"/>.</returns>
        private StageEntry ReadEntry()
        {
            var entry = new StageEntry();

            entry.Name = JusText.ReadIndirectString(reader);
            entry.Unk1 = reader.ReadInt16();
            entry.Unk2 = reader.ReadInt16();

            return entry;
        }
    }
}
