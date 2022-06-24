namespace JUSToolkit.Formats.ALAR
{
    using log4net;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class ALAR3 : IFormat
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

        public ALAR3()
        {
            AlarFiles = new NodeContainerFormat();
        }

        public void InsertModification(Node filesToInsert)
        {

            foreach (Node nNew in filesToInsert.Children)
            {
                uint newOffset = 0;
                foreach (Node nOld in Navigator.IterateNodes(AlarFiles.Root)) {

                    if (!nOld.IsContainer)
                    {
                        ALAR3File alarFileOld = nOld.GetFormatAs<ALAR3File>();

                        if (newOffset > 0)
                        {
                            alarFileOld.Offset = newOffset;
                            newOffset = alarFileOld.Offset + alarFileOld.Size;
                        }
                        if (nOld.Name == nNew.Name)
                        {
                            log.Debug("Overriding " + nNew.Name);

                            alarFileOld = ReplaceStream(alarFileOld, nNew.Stream);

                            newOffset = alarFileOld.Offset + alarFileOld.Size;
                        }
                    }
                    
                }
            }
        }

        private ALAR3File ReplaceStream(ALAR3File old, DataStream stream)
        {
            ALAR3File newAlar = new ALAR3File(stream)
            {
                FileID = old.FileID,
                Offset = old.Offset,
                Unk3 = old.Unk3,
                Unk4 = old.Unk4,
                Unk5 = old.Unk5,
                Unk6 = old.Unk6,
                Size = (uint)stream.Length
            };

            return newAlar;
        }

    }
}
