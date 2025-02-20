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
        public static readonly Version SupportedVersion = new(2, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="Alar2" /> class with an empty array of IDs.
        /// </summary>
        /// <param name="numFiles">How many files are we storing.</param>
        public Alar2(ushort numFiles)
        {
            NumFiles = numFiles;
            IDs = new byte[8];
        }

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
        /// <param name="filesToInsert">NodeContainerFormat with multiple files.</param>
        public void InsertModification(NodeContainerFormat filesToInsert)
        {
            foreach (Node nNew in Navigator.IterateNodes(filesToInsert.Root)) {
                if (!nNew.IsContainer) {
                    Console.WriteLine("Inserting " + nNew.Name);
                    InsertModification(nNew);
                }
            }
        }

        /// <summary>
        /// Inserts a new Node into the current Alar2 Container.
        /// </summary>
        /// <param name="filesToInsert">Alar2 NodeContainerFormat.</param>
        public void InsertModification(Node filesToInsert)
        {
            foreach (Node nNew in filesToInsert.Children) {
                uint nextFileOffset = 0;

                foreach (Node nOld in Navigator.IterateNodes(Root)) {
                    if (!nOld.IsContainer) {
                        Alar2File alarFileOld = nOld.GetFormatAs<Alar2File>();

                        // Ignoring first file (0 offset)
                        if (nextFileOffset > 0) {
                            alarFileOld.Offset = nextFileOffset;
                        }

                        if (nOld.Name == nNew.Name) {
                            alarFileOld.ReplaceStream(nNew.Stream);
                        }

                        nextFileOffset = alarFileOld.Offset + alarFileOld.Size;
                    }
                }
            }
        }
    }
}
