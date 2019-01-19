using System;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Converters.Bin
{
    public class Binary2BinFilename : IConverter<BinaryFormat, BinFilename>
    {

        public BinFilename Convert(BinaryFormat source)
        {

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var bin = new BinFilename();

            DataReader reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };


            // Guardamos estos dos para compararlos y sacar el tipo, no haría falta
            long currentPosition = reader.Stream.Position;
            int FirstPointer = reader.ReadInt32();
            int secondPointer = reader.ReadInt32();
            reader.Stream.Position = currentPosition;

            // Vamos al primer puntero (donde está la primera palabra)
            reader.Stream.Position = FirstPointer;
            this.ReadStringsAddingOffset(reader);
            // Volvemos al principio y leemos los punteros
            reader.Stream.Position = 0;
            this.ReadPointers(reader);
                    

            return bin;
        }

        /// <summary>
        /// Read all the strings until the end of the stream
        /// and save them into Text Dictionary with its additional Length +1 (null char).
        /// ActualPointer saves up the total length.
        /// </summary>
        private void ReadStringsAddingLength(DataReader fileToExtractReader)
        {
            int actualPointer = 0;
            while (!fileToExtractReader.Stream.EndOfStream)
            {
                string sentence = fileToExtractReader.ReadString();
                actualPointer += sentence.Length + 1; // \0 char
                //Text.Add(sentence, actualPointer);
            }

        }

        /// <summary>
        /// Read all the strings until the end of the stream
        /// and save them into Text Dictionary adding the offset
        /// </summary>
        private void ReadStringsAddingOffset(DataReader fileToExtractReader)
        {
            int offset = 0;
            //int basePointer = this.FirstPointer;

            while (!fileToExtractReader.Stream.EndOfStream)
            {
                string sentence = fileToExtractReader.ReadString();

                //Text.Add(sentence, basePointer - offset);

                //basePointer += sentence.Length + 1;
                offset += 4;
            }

        }

        /// <summary>
        /// Read all the pointers until the firstPointer
        /// and save them into Pointers Dictionary with its offset
        /// </summary>
        private void ReadPointers(DataReader fileToExtractReader)
        {
            //while (fileToExtractReader.Stream.Position < FirstPointer)
            //{
            //    int offset = (int)fileToExtractReader.Stream.Position;
            //    int pointer = fileToExtractReader.ReadInt32();

            //    if (Text.ContainsValue(pointer))
            //    {
            //        Pointers.Enqueue(pointer);
            //        Offsets.Enqueue(offset);
            //    }
            //    else
            //        FillPointers.Enqueue(pointer);
            //}
        }

    }
}
