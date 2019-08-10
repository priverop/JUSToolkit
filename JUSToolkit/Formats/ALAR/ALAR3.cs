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

        public ALAR3()
        {
            AlarFiles = new NodeContainerFormat();
        }

        public void InsertModification(Node filesToInsert)
        {

            foreach (Node nNew in filesToInsert.Children)
            {
                uint newOffset = 0;
                foreach (Node n in Navigator.IterateNodes(AlarFiles.Root)) {

                    if (!n.IsContainer)
                    {
                        ALAR3File alarFile = n.GetFormatAs<ALAR3File>();

                        if (newOffset > 0)
                        {
                            alarFile.Offset = newOffset;
                            newOffset = alarFile.Offset + alarFile.Size;
                        }
                        if (n.Name == nNew.Name)
                        {
                            log.Debug("Overriding " + nNew.Name);
                            Node newNode = new Node(nNew.Name, new ALAR3File(nNew.Stream));

                            //***AlarFiles.Children[AlarFiles.Children.IndexOf(n)] = newNode;
                            n.GetFormatAs<ALAR3File>().Size = (uint)newNode.Stream.Length;

                            newOffset = alarFile.Offset + alarFile.Size;
                        }
                    }
                    
                }
            }
        }

    }
}
