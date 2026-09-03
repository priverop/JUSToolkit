using Yarhl.IO;

namespace JUS.Tool.Containers
{
    /// <summary>
    /// Represents a children in an ALAR container.
    /// </summary>
    public class AlarFile : IBinary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlarFile"/> class passing a DataStream.
        /// </summary>
        /// <param name="fileStream">DataStream.</param>
        public AlarFile(DataStream fileStream)
        {
            Stream = fileStream;
        }

        /// <inheritdoc/>
        public Stream Stream { get; set; }

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
