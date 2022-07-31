using System;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Containers
{
    /// <summary>
    /// Alar3 Container Format.
    /// </summary>
    public class Alar3 : NodeContainerFormat
    {
        /// <summary>
        /// The Magic ID of the file.
        /// </summary>
        public const string STAMP = "ALAR";

        /// <summary>
        /// The Version of the File.
        /// </summary>
        /// <remarks>Maybe we need to support more than one minor version, but right now.
        /// I only found the 05.</remarks>
        public static readonly Version SupportedVersion = new (3, 5);

        /// <summary>
        /// Initializes a new instance of the <see cref="Alar3" /> class with an empty list of <see cref="Alar3File" />.
        /// </summary>
        /// <param name="numFiles">How many files are we storing.</param>
        public Alar3(uint numFiles)
        {
            NumFiles = numFiles;
            FileInfoPointers = new ushort[numFiles];
        }

        /// <summary>
        /// Gets the Number of files in the container.
        /// </summary>
        public uint NumFiles { get; private set; }

        /// <summary>
        /// Gets or sets the Reserved section of the container.
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// Gets or sets the Number of files - 1 in the container.
        /// </summary>
        public uint NumEntries { get; set; }

        /// <summary>
        /// Gets or sets the ending of the pointer section and the start of the file data.
        /// </summary>
        public ushort DataOffset { get; set; }

        /// <summary>
        /// Gets or sets the pointers of the file info.
        /// </summary>
        public ushort[] FileInfoPointers { get; set; }

        /// <summary>
        /// Inserts a new Node into the current Alar3 Container.
        /// </summary>
        /// <param name="filesToInsert">Alar2 NodeContainerFormat.</param>
        public void InsertModification(Node filesToInsert)
        {
            foreach (Node nNew in filesToInsert.Children) {
                uint newOffset = 0;

                foreach (Node nOld in Navigator.IterateNodes(Root)) {
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
                Size = (uint)stream.Length,
                Unknown = old.Unknown,
                Unknown2 = old.Unknown2,
                Unknown3 = old.Unknown3,
                Unknown4 = old.Unknown4,
            };

            return newAlar;
        }
    }
}
