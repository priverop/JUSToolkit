using Yarhl.IO;

namespace JUS.Tool.Containers
{
    /// <summary>
    /// Single file of an Alar3 Container.
    /// </summary>
    public class Alar3File : IBinary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alar3File"/> class passing a DataStream.
        /// </summary>
        /// <param name="fileStream">DataStream.</param>
        public Alar3File(DataStream fileStream)
        {
            Stream = fileStream;
        }

        /// <inheritdoc/>
        public DataStream Stream { get; private set; }

        /// <summary>
        /// Gets or sets the FileID.
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
        /// Gets or sets flags about the file content.
        /// </summary>
        public uint Flags { get; set; }

        /// <summary>
        /// Gets or sets the lookup hash of the filename.
        /// </summary>
        public ushort FilenameHash { get; set; }

        /// <summary>
        /// We replace the Alar3File Stream and the Size.
        /// </summary>
        /// <param name="stream">New DataStream.</param>
        public void ReplaceStream(DataStream stream)
        {
            Stream = new DataStream(stream);
            Size = (uint)stream.Length;
        }
    }
}
