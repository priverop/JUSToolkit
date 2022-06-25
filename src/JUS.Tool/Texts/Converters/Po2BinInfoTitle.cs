// Copyright (c) 2022 Pablo Rivero
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Collections.Generic;
using System.Linq;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between BinInfoTitle and Po.
    /// </summary>
    public class Po2BinInfoTitle : IConverter<Po, BinInfoTitle>
    {
        /// <summary>
        /// Gets or sets the OriginalFile property, an auxiliar File.
        /// </summary>
        public DataReader OriginalFile { get; set; }

        /// <summary>
        /// What is this @Darkc0m?.
        /// </summary>
        /// <param name="reader">DataReader reader.</param>
        public static void Go2Text(DataReader reader)
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

        /// <summary>
        /// Converts a Po Node into a BinInfoTitle Node.
        /// </summary>
        /// <param name="source">Po Node.</param>
        /// <returns>BinInfoTitle Node.</returns>
        public BinInfoTitle Convert(Po source)
        {
            int pointerValue;
            int offset;
            int updatedOffset;
            var transformOffset = new Dictionary<int, int>();
            string sentence;
            var bin = new BinInfoTitle();

            Go2Text(OriginalFile);

            int textOffset = (int)OriginalFile.Stream.Position;

            // El primer offset no varia
            updatedOffset = textOffset;

            // Lleno el diccionario para actualizar los offsets y la lista para el nuevo texto
            for (int i = 0; i < source.Entries.Count; i++)
            {
                // Almaceno en el diccionario el offset original y su nuevo offset
                offset = (int)OriginalFile.Stream.Position;
                transformOffset.Add(offset, updatedOffset);

                // Guardo el texto
                sentence = source.Entries[i].Text;
                if (sentence.Equals("<!empty>")) {
                    sentence = string.Empty;
                }

                bin.Text.Add(sentence);

                // Calculo el valor del siguiente offset nuevo
                updatedOffset += OriginalFile.DefaultEncoding.GetByteCount(sentence) + 1;

                // Me muevo a la siguiente cadena
                _ = OriginalFile.ReadString();
            }

            // Vuelvo al comienzo del fichero para escribir los nuevos punteros
            OriginalFile.Stream.Position = 0x00;

            // Recorro todos los punteros
            for (int i = 0; i < textOffset / 2; i++)
            {
                pointerValue = OriginalFile.ReadInt16();

                // Calculo la posicion absoluta a partir de la posicion del puntero (position - 2) mas su valor
                offset = (int)OriginalFile.Stream.Position - 2 + pointerValue;

                // Cambio ese offset por el recalculado si existe en el diccionario
                if (transformOffset.ContainsKey(offset))
                {
                    offset = transformOffset[offset];
                    pointerValue = offset - (i * 2);
                }

                bin.Pointers.Add(pointerValue);
            }

            return bin;
        }
    }
}
