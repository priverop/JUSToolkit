namespace JUSToolkit.Formats
{
    public class ALAR2 : ALAR
    {
        public char[] Header { get; set; }
        public byte Type { get; set; }
        public byte Unk { get; set; }
        public ushort Num_files { get; set; }
        public byte[] IDs { get; set; }
    }
}
