namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a JGalaxySimple file.
    /// </summary>
    public class JGalaxySimpleEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 164;

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Unknown section.
        /// </summary>
        public byte[] Unknown { get; set; }
    }
}
