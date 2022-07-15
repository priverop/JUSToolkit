using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUS.Tool.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;
using static System.Net.Mime.MediaTypeNames;

namespace JUS.Tool.Texts.Converters
{
    public class Binary2Bgm : IConverter<BinaryFormat, Bgm>, IConverter<Bgm, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Bgm Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var bgm = new Bgm();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            bgm.Count = reader.ReadInt32();
            for (int i = 0; i < bgm.Count; i++) {
                bgm.Entries.Add(ReadEntry());
            }

            return bgm;
        }

        public BinaryFormat Convert(Bgm bgm)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText((BgmEntry.EntrySize * bgm.Count) + 0x04);

            writer.Write(bgm.Count);

            foreach (BgmEntry entry in bgm.Entries) {
                JusText.WriteStringPointer(entry.Title, writer, jit);
                JusText.WriteStringPointer(entry.Desc1, writer, jit);
                JusText.WriteStringPointer(entry.Desc2, writer, jit);
                JusText.WriteStringPointer(entry.Desc3, writer, jit);
                writer.Write(entry.Unk1);
                writer.Write(entry.Unk2);
                writer.Write(entry.Icon);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="BgmEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="BgmEntry"/>.</returns>
        private BgmEntry ReadEntry()
        {
            var entry = new BgmEntry();

            entry.Title = JusText.ReadIndirectString(reader);
            entry.Desc1 = JusText.ReadIndirectString(reader);
            entry.Desc2 = JusText.ReadIndirectString(reader);
            entry.Desc3 = JusText.ReadIndirectString(reader);
            entry.Unk1 = reader.ReadInt16();
            entry.Unk2 = reader.ReadInt16();
            entry.Icon = reader.ReadInt32();

            return entry;
        }
    }
}
