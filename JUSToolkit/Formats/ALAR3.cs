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


        // ** O QUIZA MEJOR UNA LISTA O ALGO CON OFFSET y FILENAME (Dictionary?)
        // Así a la hora de hacer el ALAR2Nodes solamente tiene que ir a esos offsets 
        // Y ya está. Teniendo Nodes quizá sea menos eficiente, no lo sé

        public ALAR3()
        {

        }
    }
}
