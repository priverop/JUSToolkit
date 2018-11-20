namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using JUSToolkit.Formats;
    using Yarhl.FileSystem;

    public class BinaryFormat2Alar2 : IConverter<BinaryFormat, ALAR2>
    {
        public ALAR2 Convert(BinaryFormat input){

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            input.Stream.Seek(0, SeekMode.Start); // Just in case

            DataReader br = new DataReader(input.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEnconding("ascii")
            };

            var aar = new ALAR2
            {
                Header = br.ReadChars(4),
                Type = br.ReadByte(),
                Unk = br.ReadByte(),
                Num_files = br.ReadUInt16(),
            };

            aar.IDs = new byte[8];
            for (int i = 0; i < 8; i++)
                aar.IDs[i] = br.ReadByte();

            // Index table
            uint name_offset = (uint)(0x10 + aar.Num_files * 0x10);
            for (int i = 0; i < aar.Num_files; i++)
            {
                uint unk1 = br.ReadUInt32();
                uint offset = br.ReadUInt32();
                uint size = br.ReadUInt32();
                uint unk2 = br.ReadUInt32();

                DataStream fileStream = new DataStream(input.Stream, offset, size);

                long curPos = br.Stream.Position;
                br.Stream.Position = name_offset + 2;
                string filename = br.ReadString();
                name_offset += size + 0x24;
                br.Stream.Position = curPos;

                aar.Files.Add(new Node(filename, new BinaryFormat(fileStream)));
            }

            br.Stream.Dispose();

            return aar;
        }
    }
}
