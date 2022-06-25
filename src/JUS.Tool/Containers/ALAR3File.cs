using Yarhl.IO;

namespace JUSToolkit.Containers
{
    /// <summary>
    /// Single file of an Alar3 Container.
    /// </summary>
    public class Alar3File : BinaryFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alar3File"/> class passing a DataStream.
        /// </summary>
        /// <param name="fileStream">DataStream.</param>
        public Alar3File(DataStream fileStream)
            : base(fileStream)
        {
        }

        /// <summary>
        /// Gets or sets the FileID.
        /// </summary>
        public ushort FileID { get; set; }

        /// <summary>
        /// Gets or sets the Unk3.
        /// </summary>
        public ushort Unk3 { get; set; }

        /// <summary>
        /// Gets or sets the Offset of the File.
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Gets or sets the Size of the File.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Gets or sets the Unk4.
        /// </summary>
        public ushort Unk4 { get; set; }

        /// <summary>
        /// Gets or sets the Unk5.
        /// </summary>
        public ushort Unk5 { get; set; }

        /// <summary>
        /// Gets or sets the Unk6.
        /// </summary>
        public ushort Unk6 { get; set; }
    }
}