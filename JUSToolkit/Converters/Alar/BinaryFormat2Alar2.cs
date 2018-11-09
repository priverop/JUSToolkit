namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using JUSToolkit.Formats;

    public class BinaryFormat2Alar2 : IConverter<BinaryFormat, ALAR2>
    {
        public BinaryFormat2Alar2()
        {
        }

        public ALAR2 Convert(BinaryFormat input){
            return new ALAR2();
        }
    }
}
