namespace JUSToolkit.Converters.Bin
{
    using System;
    using System.Text;
    using JUSToolkit.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Binary2BinQuiz : IConverter<BinaryFormat, BinQuiz>
    {

        public BinQuiz Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.GetEncoding("shift_jis")
            };

            reader.Stream.Position = 0;

            var bin = new BinQuiz
            {
                Uknown = reader.ReadInt32(),
                Uknown2 = reader.ReadInt32(),
                FirstPointer = reader.ReadInt32(),
            };

            // Vamos al primer puntero (donde está la primera palabra)
            reader.Stream.Position = bin.FirstPointer;
            this.ReadStringsAddingOffset(bin, reader);
            // Volvemos al principio y leemos los punteros
            reader.Stream.Position = 0;
            this.ReadPointers(bin, reader);

            return bin;

        }

        /// <summary>
        /// Read all the strings until the end of the stream
        /// and save them into Text Dictionary adding the offset
        /// </summary>
        private void ReadStringsAddingOffset(BinQuiz bin, DataReader reader)
        {
            int offset = 8;
            int basePointer = bin.FirstPointer + offset;
            reader.Stream.Position = basePointer;

            while (!reader.Stream.EndOfStream)
            {
                string sentence = reader.ReadString();

                bin.Text.Add(sentence, basePointer);

                offset += 4;

                basePointer += sentence.Length + 1 + offset;
            }

        }

        /// <summary>
        /// Read all the pointers until the firstPointer
        /// and save them into Pointers Dictionary with its offset
        /// </summary>
        private void ReadPointers(BinQuiz bin, DataReader reader)
        {
            while (reader.Stream.Position < bin.FirstPointer)
            {
                int offset = (int)reader.Stream.Position;
                int pointer = reader.ReadInt32();

                if (bin.Text.ContainsValue(pointer))
                {
                    bin.Pointers.Enqueue(pointer);
                    bin.Offsets.Enqueue(offset);
                }
                else
                    bin.FillPointers.Enqueue(pointer);
            }
        }


    }
}
