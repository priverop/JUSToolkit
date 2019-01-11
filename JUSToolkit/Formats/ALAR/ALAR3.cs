namespace JUSToolkit.Formats.ALAR
{
    using System.Collections.Generic;
    using Yarhl.FileFormat;

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
            for (int i = 0; i < AlarFiles.Count; i++)
            {
                foreach (ALAR3File n in newAlar.AlarFiles)
                {
                    if (n.File.Name == AlarFiles[i].File.Name)
                    {
                        AlarFiles[i] = n;
                    }
                }
            }
        }

    }
}
