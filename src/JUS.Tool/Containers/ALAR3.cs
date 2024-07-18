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
        /// We need to iterate the whole ALAR to adjust the pointers (offsets).
        /// <param name="nNew">Node to insert.</param>
        /// <param name="path">Path of the file to replace (insert).</param>
        public void InsertModification(Node nNew, string parent = null)
        {
            uint nextFileOffset = 0;

            foreach (Node nOld in Navigator.IterateNodes(Root)) {
                if (!nOld.IsContainer) {
                    Alar3File alarFileOld = nOld.GetFormatAs<Alar3File>();

                    // Ignoring first file (0 offset)
                    if (nextFileOffset > 0) {
                        alarFileOld.Offset = nextFileOffset;
                    }

                    if (parent == null && nOld.Name == nNew.Name) {
                        alarFileOld.ReplaceStream(nNew.Stream);
                    }

                    // Search for the specific file in case there are more than one in different directories
                    // That's why specify the parent (directory name)
                    else if (parent != null && parent == nOld.Parent.Name && nOld.Name == nNew.Name) {
                        alarFileOld.ReplaceStream(nNew.Stream);
                    }

                    nextFileOffset = alarFileOld.Offset + alarFileOld.Size;
                }
            }
        }
    }
}
