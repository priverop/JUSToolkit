using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Converters.Bin
{
    class BinInfoTitle2Bin : IConverter<BinInfoTitle, BinaryFormat>
    {
        public BinaryFormat Convert(BinInfoTitle source)
        {

            var binary = new BinaryFormat();

            DataWriter writer = new DataWriter(binary.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };

            int offset = source.Text.Count * 4;

            for(int i = 0; i < source.Text.Count; i++)
            {
                writer.WriteOfType<Int32>(offset);
                offset = offset - 4 + writer.DefaultEncoding.GetByteCount(source.Text.ElementAt(i));
            }

            foreach(string sentence in source.Text)
            {
                writer.Write(sentence);
            }

            return binary;
        }
    }
}
