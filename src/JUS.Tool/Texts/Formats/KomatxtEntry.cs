namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a Komatxt file.
    /// </summary>
    public class KomatxtEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0xC;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public int Unk1 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public int Unk2 { get; set; }
    }
}
