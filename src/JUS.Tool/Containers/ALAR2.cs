using System;
using Yarhl.FileSystem;

namespace JUSToolkit.Containers
{
    /// <summary>
    /// Alar2 Container Format.
    /// </summary>
    public class Alar2 : NodeContainerFormat
    {
        /// <summary>
        /// The Magic ID of the file.
        /// </summary>
        public const string STAMP = "ALAR";

        /// <summary>
        /// The Version of the File.
        /// </summary>
        /// <remarks>Maybe we need to support more than one minor version, but right now.
        /// I only found the 01.</remarks>
        public static readonly Version SupportedVersion = new (2, 1);

        /// <summary>
        /// Gets or sets the Number of files in the container.
        /// </summary>
        public ushort NumFiles { get; set; }

        /// <summary>
        /// Gets or sets the IDs of the files.
        /// </summary>
        public byte[] IDs { get; set; }
    }
}
