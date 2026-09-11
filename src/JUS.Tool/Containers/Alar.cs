using Yarhl.FileSystem;

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
        /// The name of the info node.
        /// </summary>
        public const string InfoNodeName = "_info";

        /// <summary>
        /// Gets the Alar container meta-information.
        /// </summary>
        public AlarInfo Info => Root.Children[InfoNodeName]?.GetFormatAs<AlarInfo>()
            ?? throw new FormatException("Missing info node");
    }
}
