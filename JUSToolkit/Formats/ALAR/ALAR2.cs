namespace JUSToolkit.Formats.ALAR
{
    using System.Collections.Generic;
    using Yarhl.FileFormat;

    public class ALAR2 : Format
    {
        public char[] Header { get; set; }
        public byte Type { get; set; }
        public byte Unk { get; set; }
        public ushort Num_files { get; set; }
        public byte[] IDs { get; set; }
        public List<ALAR2File> AlarFiles { get; set; }

        public ALAR2()
        {
            AlarFiles = new List<ALAR2File>();
        }

        public void InsertModification(ALAR2 newAlar)
        {

            for (int i = 0; i < AlarFiles.Count; i++)
            {
                foreach (ALAR2File n in newAlar.AlarFiles)
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
