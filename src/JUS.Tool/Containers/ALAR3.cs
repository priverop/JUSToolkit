using System;
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
        /// I only found the 05 and the 69.</remarks>
        public static readonly Version[] SupportedVersions = new Version[] { new Version(3, 5), new Version(3, 69) };

        /// <summary>
        /// Initializes a new instance of the <see cref="Alar3" /> class with an empty array of FileInfoPointers.
        /// </summary>
        /// <param name="numFiles">How many files are we storing.</param>
        public Alar3(uint numFiles)
        {
            NumFiles = numFiles;
            FileInfoPointers = new ushort[numFiles];
        }

        /// <summary>
        /// Gets or sets the Minor version of the container.
        /// </summary>
        public byte MinorVersion { get; set; }

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
        /// <param name="filesToInsert">NodeContainerFormat with multiple files.</param>
        public void InsertModification(NodeContainerFormat filesToInsert)
        {
            foreach (Node nNew in filesToInsert.Root.Children) {
                InsertModification(nNew);
            }
        }

        /// <summary>
        /// Inserts a new Node into the current Alar3 Container.
        /// </summary>
        /// <param name="nNew">Node to insert.</param>
        public void InsertModification(Node nNew)
        {
            uint nextFileOffset = 0;
            bool found = false; // infodeck.aar has the same file twice (different folders)... maybe we need to redo this

            foreach (Node nOld in Navigator.IterateNodes(Root)) {
                if (!nOld.IsContainer) {
                    Alar3File alarFileOld = nOld.GetFormatAs<Alar3File>();

                    // Ignoring first file (0 offset)
                    if (nextFileOffset > 0) {
                        alarFileOld.Offset = nextFileOffset;
                    }

                    if (nOld.Name == nNew.Name && !found) {
                        alarFileOld.ReplaceStream(nNew.Stream);
                        found = true;
                    }

                    nextFileOffset = alarFileOld.Offset + alarFileOld.Size;
                }
            }
        }
    }
}
