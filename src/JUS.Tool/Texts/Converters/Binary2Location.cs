using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    public class Binary2Location :
        IConverter<BinaryFormat, Location>,
        IConverter<Location, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        public Location Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var location = new Location();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            location.Count = reader.ReadInt32();

            for (int i = 0; i < location.Count; i++) {
                location.Entries.Add(ReadEntry());
            }

            return location;
        }

        public BinaryFormat Convert(Location location)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText((LocationEntry.EntrySize * location.Count) + 0x04);

            writer.Write(location.Count);

            foreach (LocationEntry entry in location.Entries) {
                JusText.WriteStringPointer(entry.Name, writer, jit);
                writer.Write(entry.Unk1);
                writer.Write(entry.Unk2);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="LocationEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="LocationEntry"/>.</returns>
        private LocationEntry ReadEntry()
        {
            var entry = new LocationEntry();

            entry.Name = JusText.ReadIndirectString(reader);
            entry.Unk1 = reader.ReadInt16();
            entry.Unk2 = reader.ReadInt16();

            return entry;
        }
    }
}
