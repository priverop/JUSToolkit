using Yarhl.IO;

namespace JUS.Tool.Containers
{
    /// <summary>
    /// Single file of an Alar2 Container.
    /// </summary>
    public class Alar2File : IBinary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Alar2File"/> class passing a DataStream.
        /// </summary>
        /// <param name="fileStream">DataStream.</param>
        public Alar2File(DataStream fileStream)
        {
            Stream = fileStream;
        }

        /// <inheritdoc/>
        public DataStream Stream { get; set; }

        /// <summary>
        /// Gets or sets the internal game identifier of the file.
        /// </summary>
        public uint FileId { get; set; }

        /// <summary>
        /// Gets or sets flags about the file content.
        /// </summary>
        public uint Flags { get; set; }
    }
}
