using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;
using JUSToolkit.Formats;
using Yarhl.IO;

namespace JUSToolkit.Converters.Bin
{
    class Binary2BinInfoTitle : IConverter<BinaryFormat, BinInfoTitle>
    {
        public BinInfoTitle Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };

            var bin = new BinInfoTitle();

            //Posicionar el reader al comienzo del texto
            int firstPointer = reader.ReadInt32();
            reader.Stream.Position = firstPointer;

            //Guardar todo el texto en bin
            while (!reader.Stream.EndOfStream)
            {
                bin.Text.Add(reader.ReadString());
            }

            return bin;
        }
    }
}
