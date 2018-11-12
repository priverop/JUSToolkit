namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using JUSToolkit.Formats;

    public class BinaryFormat2Alar2 : IConverter<BinaryFormat, ALAR2>
    {
        public ALAR2 Convert(BinaryFormat input){

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            input.Stream.Seek(0, SeekMode.Start); // Just in case

            DataReader br = new DataReader(input.Stream);

            return new ALAR2();
        }
    }
}
