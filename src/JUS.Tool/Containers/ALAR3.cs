using JUS.Tool.Utils;
using Yarhl.FileSystem;

namespace JUS.Tool.Containers
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
        /// The supported feature flags of this tool.
        /// </summary>
        public static readonly byte[] SupportedFeatureFlags = [ 0x05, 0x45 ];

        /// <summary>
        /// Gets or sets the container feature flags.
        /// </summary>
        public byte FeatureFlags { get; set; }

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
        /// Inserts a new Node into the current Alar3 Container.
        /// </summary>
        /// We need to iterate the whole ALAR to adjust the pointers (offsets).
        /// <param name="nNew">Node to insert.</param>
        /// <param name="parent">Parent directory of the file to replace.</param>
        public void InsertModification(Node nNew, string? parent = null)
        {
            uint nextFileOffset = 0;
            bool replaced = false;

            foreach (Node nOld in Navigator.IterateNodes(Root)) {
                if (!nOld.IsContainer) {
                    Alar3File alarFileOld = nOld.GetFormatAs<Alar3File>()!;

                    // Ignoring first file (0 offset)
                    if (nextFileOffset > 0) {
                        alarFileOld.Offset = nextFileOffset;
                    }

                    if (parent == null && nOld.Name == nNew.Name) {
                        Console.WriteLine("Replacing: " + nNew.Name);
                        alarFileOld.ReplaceStream(nNew.Stream!);
                        replaced = true;
                    }

                    // Search for the specific file in case there are more than one in different directories
                    // That's why specify the parent (directory name)
                    else if (parent != null && parent == nOld.Parent!.Name && nOld.Name == nNew.Name) {
                        alarFileOld.ReplaceStream(nNew.Stream!);
                        replaced = true;
                    }

                    nextFileOffset = alarFileOld.Offset + alarFileOld.Size;
                }
            }

            if (!replaced) {
                Logger.DisplayError($"❌ {nNew.Name} node not found in the container");
            }
        }
    }
}
