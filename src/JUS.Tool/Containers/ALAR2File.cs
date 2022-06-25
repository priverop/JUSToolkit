using Yarhl.FileSystem;

namespace JUSToolkit.Formats.ALAR
{
    /// <summary>
    /// Single file of an Alar2 Container.
    /// </summary>
    public class Alar2File
    {
        /// <summary>
        /// Gets or sets the File of the Node.
        /// </summary>
        public Node File { get; set; }

        /// <summary>
        /// Gets or sets the Unknown1.
        /// </summary>
        public uint Unknown1 { get; set; }

        /// <summary>
        /// Gets or sets the Unknown2.
        /// </summary>
        public uint Unknown2 { get; set; }
    }
}
