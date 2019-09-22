using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;
using Yarhl.IO;
using System.Linq;
using System.Collections.Generic;

namespace JUSToolkit.Converters.Bin
{
    class Po2BinInfoTitle : IConverter<Po, BinInfoTitle>
    {
        public DataReader OriginalFile { get; set; }

        public BinInfoTitle Convert(Po source)
        {
            int pointerValue;
            int offset;
            int updatedOffset;
            Dictionary<int, int> transformOffset = new Dictionary<int, int>();
            string sentence;
            var bin = new BinInfoTitle();

            Go2Text(OriginalFile);

            int textOffset = (int)OriginalFile.Stream.Position;

            //El primer offset no varia
            updatedOffset = textOffset;

            //Lleno el diccionario para actualizar los offsets y la lista para el nuevo texto
            for (int i = 0; i < source.Entries.Count; i++)
            {
                //Almaceno en el diccionario el offset original y su nuevo offset
                offset = (int)OriginalFile.Stream.Position;
                transformOffset.Add(offset, updatedOffset);

                //Guardo el texto
                sentence = source.Entries.ElementAt(i).Text;
                if (sentence.Equals("<!empty>"))
                    sentence = string.Empty;
                bin.Text.Add(sentence);

                //Calculo el valor del siguiente offset nuevo
                updatedOffset += OriginalFile.DefaultEncoding.GetByteCount(sentence) + 1;

                //Me muevo a la siguiente cadena
                OriginalFile.ReadString();
            }

            //Vuelvo al comienzo del fichero para escribir los nuevos punteros          
            OriginalFile.Stream.Position = 0x00;

            //Recorro todos los punteros
            for (int i = 0; i < textOffset / 2; i++)
            {
                pointerValue = OriginalFile.ReadInt16();

                //Calculo la posicion absoluta a partir de la posicion del puntero (position - 2) mas su valor            
                offset = (int)OriginalFile.Stream.Position - 2 + pointerValue;

                //Cambio ese offset por el recalculado si existe en el diccionario
                if (transformOffset.ContainsKey(offset))
                {
                    offset = transformOffset[offset];
                    pointerValue = offset - i * 2;
                }

                bin.Pointers.Add(pointerValue);
            }

            return bin;

        }

        public void Go2Text(DataReader reader)
        {
            reader.Stream.Position = 0x00;
            int pointerValue = reader.ReadInt16();

            switch (pointerValue)
            {
                case 0x0029:
                case 0x005B:
                case 0x0034:
                case 0x0032:
                case 0x0D04:
                case 0x0059:
                    reader.Stream.Position += 2;
                    pointerValue = reader.ReadInt16();
                    break;
                default:
                    break;
            }

            reader.Stream.Position += pointerValue - 2;

        }
    }
}
