using System;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Containers
{
    /// <summary>
    /// Alar2 Container Format.
    /// </summary>
    public class Alar2 : NodeContainerFormat
    {
        /// <summary>
        /// The Magic ID of the file.
        /// </summary>
        public const string STAMP = "ALAR";

        /// <summary>
        /// The Version of the File.
        /// </summary>
        /// <remarks>Maybe we need to support more than one minor version, but right now.
        /// I only found the 01.</remarks>
        public static readonly Version SupportedVersion = new (2, 1);

        /// <summary>
        /// Gets or sets the Number of files in the container.
        /// </summary>
        public ushort NumFiles { get; set; }

        /// <summary>
        /// Gets or sets the IDs of the files.
        /// </summary>
        public byte[] IDs { get; set; }

        /// <summary>
        /// Inserts a new Node into the current Alar2 Container.
        /// </summary>
        /// <param name="filesToInsert">Alar2 NodeContainerFormat.</param>
        public void InsertModification(Node filesToInsert)
        {
            foreach (Node nNew in filesToInsert.Children) {
                uint newOffset = 0;

                foreach (Node nOld in Navigator.IterateNodes(Root)) {
                    if (!nOld.IsContainer) {
                        Alar2File alarFileOld = nOld.GetFormatAs<Alar2File>();

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
        /// Replaces an Alar2File with a Datastream.
        /// </summary>
        /// <param name="old">Alar2File old File.</param>
        /// /// <param name="stream">New DataStream.</param>
        private static Alar2File ReplaceStream(Alar2File old, DataStream stream)
        {
            var newAlar = new Alar2File(stream) {
                FileID = old.FileID,
                Offset = old.Offset,
                Size = (uint)stream.Length,
                Unknown = old.Unknown,
                Unknown2 = old.Unknown2,
            };

            return newAlar;
        }
    }
}
