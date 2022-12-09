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
    public class Binary2Pname :
        IConverter<BinaryFormat, Pname>,
        IConverter<Pname, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        public Pname Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var pname = new Pname();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            pname.Count = reader.ReadInt32();

            for (int i = 0; i < pname.Count; i++) {
                pname.TextEntries.Add(JusText.ReadIndirectString(reader));
            }

            return pname;
        }

        public BinaryFormat Convert(Pname pname)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText(SimpleBin.EntrySize * pname.Count + 0x04);

            writer.Write(pname.Count);
            foreach (string entry in pname.TextEntries) {
                JusText.WriteStringPointer(entry, writer, jit);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }
    }
}
