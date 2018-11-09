namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using JUSToolkit.Formats;
    using Yarhl.FileSystem;
    using log4net;

    public class BinaryFormat2Alar3 : IConverter<BinaryFormat, ALAR3>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BinaryFormat2Alar3));

        public BinaryFormat2Alar3()
        {
        }

        public ALAR3 Convert(BinaryFormat input){

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            input.Stream.Seek(0, SeekMode.Start); // Just in case

            DataReader br = new DataReader(input.Stream);

            var aar = new ALAR3
            {
                Header = br.ReadChars(4),
                Type = br.ReadByte(),
                Unk = br.ReadByte(),
                Num_files = br.ReadUInt32(),
                Unk2 = br.ReadUInt16(),
                Array_count = br.ReadUInt32(),
                EndFileIndex = br.ReadUInt16()
            };
            aar.FileTableIndex = new ushort[aar.Array_count + 1];

            for (int i = 0; i < aar.Array_count + 1; i++)
            {
                aar.FileTableIndex[i] = br.ReadUInt16();
            }

            // Index table
            foreach (ushort filePosition in aar.FileTableIndex)
            {
                br.Stream.Position = filePosition;

                ushort fileID = br.ReadUInt16();
                ushort unk3 = br.ReadUInt16();
                uint offset = br.ReadUInt32();
                uint size = br.ReadUInt32();

                DataStream fileStream = new DataStream(input.Stream, offset, size);

                ushort unk4 = br.ReadUInt16();
                ushort unk5 = br.ReadUInt16();
                ushort unk6 = br.ReadUInt16();

                string filename = br.ReadString().Replace("/", "-");

                aar.Files.Add(new Node(filename, new BinaryFormat(fileStream)));
            }

            br.Stream.Dispose();

            return aar;
        }
    }
}
