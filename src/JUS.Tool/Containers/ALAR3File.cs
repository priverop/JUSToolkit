using Yarhl.IO;

namespace JUSToolkit.Containers
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
        public ushort FileID { get; set; }

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
        public ushort Unknown { get; set; }

        /// <summary>
        /// Gets or sets the Unknown2.
        /// </summary>
        public ushort Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the Unknown3.
        /// </summary>
        public ushort Unknown3 { get; set; }

        /// <summary>
        /// Gets or sets the Unknown4.
        /// </summary>
        public ushort Unknown4 { get; set; }

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