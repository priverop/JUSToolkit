using System;
using System.Collections.Generic;
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
        /// Supported Versions of the File.
        /// </summary>
        /// <remarks>I only found 1 and 3.</remarks>
        public static readonly List<Version> SupportedVersions = new()
        {
            new Version(2, 1),
            new Version(2, 3),
        };

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
        /// Gets or sets the Minor Version of the container.
        /// </summary>
        public int MinorVersion { get; set; }

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
        /// <param name="nNew">Node to insert.</param>
        /// <param name="parent">Parent directory of the file to replace.</param>
        public void InsertModification(Node nNew, string parent = null)
        {
            uint nextFileOffset = 0;
            bool replaced = false;

            foreach (Node nOld in Navigator.IterateNodes(Root)) {
                if (!nOld.IsContainer) {
                    Alar2File alarFileOld = nOld.GetFormatAs<Alar2File>();

                    // Ignoring first file (0 offset)
                    if (nextFileOffset > 0) {
                        alarFileOld.Offset = nextFileOffset;
                    }

                    if (parent == null && nOld.Name == nNew.Name) {
                        Console.WriteLine("Replacing: " + nNew.Name);
                        alarFileOld.ReplaceStream(nNew.Stream);
                        replaced = true;
                    }

                    // Search for the specific file in case there are more than one in different directories
                    // That's why specify the parent (directory name)
                    else if (parent != null && parent == nOld.Parent.Name && nOld.Name == nNew.Name) {
                        alarFileOld.ReplaceStream(nNew.Stream);
                    }

                    nextFileOffset = alarFileOld.Offset + alarFileOld.Size;
                }
            }

            if (!replaced) {
                Console.WriteLine($"{nNew.Name} node not found in the container");
            }
        }
    }
}
