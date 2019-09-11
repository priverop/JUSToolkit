using System;
using System.Linq;
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

            //Calculo del puntero inicial
            int offset = source.Text.Count * 4;

            //Escritura de cada uno de los punteros
            for(int i = 0; i < source.Text.Count; i++)
            {
                writer.WriteOfType<Int32>(offset);
                offset = offset - 4 + writer.DefaultEncoding.GetByteCount(source.Text.ElementAt(i));
            }

            //Escritura del texto
            foreach(string sentence in source.Text)
            {
                writer.Write(sentence);
            }

            return binary;
        }
    }
}
