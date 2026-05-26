using JUS.Tool.Utils;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUS.Tool.Containers
{
    /// <summary>
    /// AL ARchive container format.
    /// </summary>
    public class Alar : NodeContainerFormat
    {
        /// <summary>
        /// The magic ID of the file.
        /// </summary>
        public const string FormatId = "ALAR";

        /// <summary>
        /// The node tag that indicates whether the container was compressed.
        /// </summary>
        public const string CompressionTag = "jus.alar.is_compressed";

        /// <summary>
        /// Gets or sets the ALAR format version.
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// Gets or sets the container feature flags.
        /// </summary>
        public AlarFormatFeatures Features { get; set; }

        /// <summary>
        /// Gets or sets the ID of the first file in the container.
        /// </summary>
        public uint FirstFileId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the last file in the container.
        /// </summary>
        public uint LastFileId { get; set; }

        /// <summary>
        /// Inserts a new Node into the current Alar3 Container.
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
        /// Inserts a new Node into the current container.
        /// </summary>
        /// We need to iterate the whole ALAR to adjust the pointers (offsets).
        /// <param name="nNew">Node to insert.</param>
        /// <param name="parent">Parent directory of the file to replace.</param>
        public void InsertModification(Node nNew, string? parent = null)
        {
            bool replaced = false;
            foreach (Node nOld in Navigator.IterateNodes(Root)) {
                if (!nOld.IsContainer) {
                    AlarFile alarFileOld = nOld.GetFormatAs<AlarFile>()!;

                    if (parent == null && nOld.Name == nNew.Name) {
                        Console.WriteLine("Replacing: " + nNew.Name);
                        alarFileOld.Stream = new DataStream(nNew.Stream!);
                        replaced = true;
                    }

                    // Search for the specific file in case there are more than one in different directories
                    // That's why specify the parent (directory name)
                    else if (parent != null && parent == nOld.Parent!.Name && nOld.Name == nNew.Name) {
                        alarFileOld.Stream = new DataStream(nNew.Stream!);
                        replaced = true;
                    }
                }
            }

            if (!replaced) {
                Logger.DisplayError($"❌ {nNew.Name} node not found in the container");
            }
        }
    }
}
