using System;
using System.Linq;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;
using System.Collections.Generic;
using System.Text;

namespace JUSToolkit.Converters.Bin
{
    class BinInfoTitle2Bin : IConverter<BinInfoTitle, BinaryFormat>
    {
        public BinaryFormat Convert(BinInfoTitle source)
        {
            var bin = new BinaryFormat();

            DataWriter writer = new DataWriter(bin.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding(932)
            };

            foreach(int pointer in source.Pointers)
            {
                writer.WriteOfType<Int16>((Int16)pointer);
            }
            foreach(string text in source.Text)
            {
                writer.Write(text);
            }

            return bin;
        }
    }
}
