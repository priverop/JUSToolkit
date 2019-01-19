namespace JUSToolkit.Converters.Bin
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using JUSToolkit.Formats;

    public class BinaryFormat2BinTutorial : IConverter<BinaryFormat, BinTutorial>
    {
       
        public BinTutorial Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };

            BinTutorial bin = new BinTutorial
            {
                FirstPointer = reader.ReadInt32()
            };

            reader.Stream.Position = bin.FirstPointer;
            this.ReadStrings(bin, reader);
            reader.Stream.Position = 4; // skip first pointer
            this.ReadPointers(bin, reader);

            return bin;

        }

        /// <summary>
        /// Read all the strings until the end of the stream
        /// and save them into Text Dictionary with its Length +1 (null char).
        /// </summary>
        private void ReadStrings(BinTutorial bin, DataReader reader)
        {
            int actualPointer = 0;
            while (!reader.Stream.EndOfStream)
            {
                string sentence = reader.ReadString();
                actualPointer += sentence.Length + 1; // \0 char
                bin.Text.Add(sentence, actualPointer);
            }

        }

        /// <summary>
        /// Read all the pointers until the firstPointer
        /// and save them into Pointers Dictionary with its offset
        /// </summary>
        private void ReadPointers(BinTutorial bin, DataReader reader)
        {
            while (reader.Stream.Position < bin.FirstPointer)
            {
                int offset = (int)reader.Stream.Position;
                int pointer = reader.ReadInt32();

                if (bin.Text.ContainsValue(pointer) && !bin.Pointers.ContainsKey(pointer))
                    bin.Pointers.Add(pointer, offset);
                else
                    bin.FillPointers.Enqueue(pointer);
            }
        }
    }
}
