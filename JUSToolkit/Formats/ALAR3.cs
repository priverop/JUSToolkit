namespace JUSToolkit.Formats
{
    using System;
    using System.Collections.Generic;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    public class ALAR3 : ALAR
    {
        public char[] Header { get; set; }
        public byte Type { get; set; }
        public byte Unk { get; set; }
        public uint Num_files { get; set; }
        public ushort Unk2 { get; set; }
        public uint Array_count { get; set; }
        public ushort EndFileIndex { get; set; }
        public ushort[] FileTableIndex { get; set; }

    }
}
