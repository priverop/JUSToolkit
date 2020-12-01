namespace JUSToolkit.Converters.Alar
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using JUSToolkit.Formats.ALAR;
    using Yarhl.FileSystem;
    using log4net;
    using JUSToolkit.Formats;
    using System.Linq;
    using System.IO;

    public class BinaryFormat2Alar3 :
        IConverter<BinaryFormat, ALAR3>,
        IConverter<ALAR3, BinaryFormat>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BinaryFormat2Alar3));

        public ALAR3 Convert(BinaryFormat input)
        {

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            input.Stream.Seek(0, SeekMode.Start); // Just in case

            DataReader br = new DataReader(input.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };

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
            aar.FileTableIndex = new ushort[aar.Array_count + 1]; //= Num_files

            for (int i = 0; i < (aar.Array_count + 1); i++)
            {
                aar.FileTableIndex[i] = br.ReadUInt16();
            }

            // Index table
            foreach (ushort filePosition in aar.FileTableIndex)
            {
                input.Stream.Position = filePosition;

                ushort FileID = br.ReadUInt16();
                ushort Unk3 = br.ReadUInt16();
                uint Offset = br.ReadUInt32();
                uint Size = br.ReadUInt32();

                DataStream fileStream = new DataStream(input.Stream, Offset, Size);

                var aarFile = new ALAR3File(fileStream)
                {
                    FileID = FileID,
                    Unk3 = Unk3,
                    Offset = Offset,
                    Size = Size,
                    Unk4 = br.ReadUInt16(),
                    Unk5 = br.ReadUInt16(),
                    Unk6 = br.ReadUInt16()
                };

                string fullFilename = br.ReadString();
                string filename = Path.GetFileName(fullFilename);
                fullFilename = fullFilename.Remove(fullFilename.IndexOf(filename, StringComparison.CurrentCulture), filename.Length);

                var child = new Node(filename, aarFile);

                NodeFactory.CreateContainersForChild(aar.AlarFiles.Root, fullFilename, child);

            }

            return aar;
        }

        public BinaryFormat Convert(ALAR3 aar)
        {
            if (aar == null)
                throw new ArgumentNullException(nameof(aar));

            BinaryFormat binary = new BinaryFormat();
            DataWriter writer = new DataWriter(binary.Stream)
            {
                DefaultEncoding = new Yarhl.Media.Text.Encodings.EscapeOutRangeEncoding("ascii")
            };

            writer.Write(aar.Header);
            writer.Write(aar.Type);
            writer.Write(aar.Unk);
            writer.Write(aar.Num_files);
            writer.Write(aar.Unk2);
            writer.Write(aar.Array_count);
            writer.Write(aar.EndFileIndex);

            for (int i = 0; i < aar.Array_count + 1; i++)
            {
                writer.Write(aar.FileTableIndex[i]);
            }

            long[] offsetPositions = new long[aar.AlarFiles.Root.Children.Count];
            int[] paddings = new int[aar.AlarFiles.Root.Children.Count];

            foreach (Node aarFile in aar.AlarFiles.Root.Children)
            {
                if (!aarFile.IsContainer)
                {
                    ALAR3File alarChild = aarFile.GetFormatAs<ALAR3File>();

                    writer.WritePadding(0, 04);

                    writer.Write(alarChild.FileID); // 2
                    writer.Write(alarChild.Unk3); // 2
                    offsetPositions[alarChild.FileID] = writer.Stream.Position;
                    writer.Write(alarChild.Offset); // 4
                    writer.Write(alarChild.Size); //4
                    writer.Write(alarChild.Unk4); //2
                    writer.Write(alarChild.Unk5); //2
                    writer.Write(alarChild.Unk6); //2

                    // ToDo
                    // directories?
                    writer.Write(aarFile.Name, true);
                }
            }

            writer.WritePadding(0, 04);

            // Primero Cabeceras y luego ficheros
            foreach (Node n in aar.AlarFiles.Root.Children)
            {
                if (!n.IsContainer)
                {
                    ALAR3File aarFile = n.GetFormatAs<ALAR3File>();

                    if (aarFile.FileID != 0)
                    {
                        long initPadding = writer.Stream.Position;
                        writer.WritePadding(0, 04);
                        long endPadding = writer.Stream.Position;
                        int paddingSize = (int)(endPadding - initPadding);

                        paddings[aarFile.FileID] = paddingSize;
                    }

                    aarFile.Stream.WriteTo(writer.Stream);
                }

            }

            // Ajustamos offsets

            int newOffset = 0;
            foreach (Node n in aar.AlarFiles.Root.Children)
            {
                if (!n.IsContainer)
                {
                    ALAR3File aarFile = n.GetFormatAs<ALAR3File>();

                    if (aarFile.FileID == 0)
                    {
                        newOffset = (int)aarFile.Offset;
                    }
                    if (aarFile.FileID != aar.AlarFiles.Root.Children.Count - 1)
                    {
                        newOffset += (int)(aarFile.Size + paddings[aarFile.FileID + 1]);
                        writer.Stream.RunInPosition(
                            () => writer.Write(newOffset),
                        offsetPositions[aarFile.FileID + 1]);
                    }
                }
            }
            return binary;
        }

    }
}
