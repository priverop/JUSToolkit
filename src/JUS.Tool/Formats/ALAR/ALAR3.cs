using log4net;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Formats.ALAR
{
    /// <summary>
    /// Alar3 Container Format.
    /// </summary>
    public class Alar3 : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alar3" /> class with an empty list of <see cref="Alar3File" />.
        /// </summary>
        public Alar3()
        {
            AlarFiles = new NodeContainerFormat();
        }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public char[] Header { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public byte Unk { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public uint Num_files { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public ushort Unk2 { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public uint Array_count { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public ushort EndFileIndex { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public ushort[] FileTableIndex { get; set; }

        /// <summary>
        /// Gets or sets the .
        /// </summary>
        public NodeContainerFormat AlarFiles { get; set; }

        /// <summary>
        /// Inserts a new Node into the current Alar3 Container.
        /// </summary>
        /// <param name="filesToInsert">Alar2 NodeContainerFormat.</param>
        public void InsertModification(Node filesToInsert)
        {
            foreach (Node nNew in filesToInsert.Children) {
                uint newOffset = 0;

                foreach (Node nOld in Navigator.IterateNodes(AlarFiles.Root)) {
                    if (!nOld.IsContainer) {
                        Alar3File alarFileOld = nOld.GetFormatAs<Alar3File>();

                        if (newOffset > 0) {
                            alarFileOld.Offset = newOffset;
                            newOffset = alarFileOld.Offset + alarFileOld.Size;
                        }

                        if (nOld.Name == nNew.Name) {
                            alarFileOld = ReplaceStream(alarFileOld, nNew.Stream);

                            newOffset = alarFileOld.Offset + alarFileOld.Size;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Replaces an Alar3File with a Datastream.
        /// </summary>
        /// <param name="old">Alar3File old File.</param>
        /// /// <param name="stream">New DataStream.</param>
        private static Alar3File ReplaceStream(Alar3File old, DataStream stream)
        {
            var newAlar = new Alar3File(stream) {
                FileID = old.FileID,
                Offset = old.Offset,
                Unk3 = old.Unk3,
                Unk4 = old.Unk4,
                Unk5 = old.Unk5,
                Unk6 = old.Unk6,
                Size = (uint)stream.Length,
            };

            return newAlar;
        }
    }
}
