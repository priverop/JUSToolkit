using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Containers
{
    /// <summary>
    /// Single file of an Alar2 Container.
    /// </summary>
    public class Alar2File : BinaryFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alar2File"/> class passing a DataStream.
        /// </summary>
        /// <param name="fileStream">DataStream.</param>
        public Alar2File(DataStream fileStream)
            : base(fileStream)
        {
        }

        /// <summary>
        /// Gets or sets the internal game identifier of the file.
        /// </summary>
        public uint FileID { get; set; }

        /// <summary>
        /// Gets or sets the absolute pointer of the File.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Gets or sets the size of the File.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Gets or sets the Unknown.
        /// </summary>
        public uint Unknown { get; set; }

        /// <summary>
        /// Gets or sets the 2 bytes previous to the Stream.
        /// </summary>
        public ushort Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the absolute number of the file.
        /// </summary>
        /// <remarks>If the container has 2 files, FileNum would be 1 or 2.</remarks>
        public int FileNum { get; set; }
    }
}
