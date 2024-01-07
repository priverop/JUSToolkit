namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a Rulemess file.
    /// </summary>
    public class RulemessEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x10;

        /// <summary>
        /// Gets or sets the Description1.
        /// </summary>
        public string Description1 { get; set; }

        /// <summary>
        /// Gets or sets the Description2.
        /// </summary>
        public string Description2 { get; set; }

        /// <summary>
        /// Gets or sets the Description3.
        /// </summary>
        public string Description3 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public int Unk1 { get; set; }
    }
}
