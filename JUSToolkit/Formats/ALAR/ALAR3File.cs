namespace JUSToolkit.Formats.ALAR
{
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class ALAR3File : BinaryFormat
    {
        public ALAR3File(DataStream fileStream) : base(fileStream)
        {
            // Fuck Java
        }
        public ushort FileID { get; set; }
        public ushort Unk3 { get; set; }
        public uint Offset { get; set; }
        public uint Size { get; set; }
        public ushort Unk4 { get; set; }
        public ushort Unk5 { get; set; }
        public ushort Unk6 { get; set; }
    }
}
