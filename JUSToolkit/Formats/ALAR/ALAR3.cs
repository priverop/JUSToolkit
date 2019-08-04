namespace JUSToolkit.Formats.ALAR
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    public class ALAR3 : Format
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Identify));

        public char[] Header { get; set; }
        public byte Type { get; set; }
        public byte Unk { get; set; }
        public uint Num_files { get; set; }
        public ushort Unk2 { get; set; }
        public uint Array_count { get; set; }
        public ushort EndFileIndex { get; set; }
        public ushort[] FileTableIndex { get; set; }
        public NodeContainerFormat AlarFiles { get; set; }

        public void InsertModification(ALAR3 newAlar)
        {

            foreach (ALAR3File newFile in newAlar.AlarFiles)
            {
                uint newOffset = 0;
                for (int i = 0; i < AlarFiles.Count; i++) {
                    if (newOffset > 0)
                    {
                        AlarFiles[i].Offset = newOffset;
                        newOffset = GetNewOffset(i);
                    }
                    if (AlarFiles[i].File.Name == newFile.File.Name)
                    {
                        log.Debug("Overriding "+ newFile.File.Name);
                        Node newNode = new Node(newFile.File.Name, new BinaryFormat(newFile.File.Stream));
                        AlarFiles[i].File = newNode;
                        AlarFiles[i].Size = (uint)newNode.Stream.Length;
                       
                        newOffset = GetNewOffset(i);
                    }
                }
            }
        }

        private uint GetNewOffset(int i)
        {
            return AlarFiles[i].Offset + AlarFiles[i].Size;
        }

    }
}
