namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a JGalaxy file.
    /// </summary>
    public class JGalaxyEntry
    {
        /// <summary>
        /// Gets or sets the entry size in bytes.
        /// </summary>
        public int EntrySize { get; set; }

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
