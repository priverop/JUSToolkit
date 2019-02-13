namespace JUSToolkit.Formats.ALAR
{
    using System.Collections.Generic;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    public class ALAR3 : Format
    {
        public char[] Header { get; set; }
        public byte Type { get; set; }
        public byte Unk { get; set; }
        public uint Num_files { get; set; }
        public ushort Unk2 { get; set; }
        public uint Array_count { get; set; }
        public ushort EndFileIndex { get; set; }
        public ushort[] FileTableIndex { get; set; }
        public List<ALAR3File> AlarFiles { get; set; }

        public ALAR3()
        {
            AlarFiles = new List<ALAR3File>();
        }

        public void InsertModification(ALAR3 newAlar)
        {
            foreach (ALAR3File newFile in newAlar.AlarFiles)
            {
                for (int i = 0; i < AlarFiles.Count; i++) {
                    if (AlarFiles[i].File.Name == newFile.File.Name)
                    {
                        Node newNode = new Node(newFile.File.Name, new BinaryFormat(newFile.File.Stream));
                        AlarFiles[i].File = newNode;
                        AlarFiles[i].Size = (uint)newNode.Stream.Length;
                    }
                }
            }
            
        }

    }
}
