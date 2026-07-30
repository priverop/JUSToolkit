namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a <see cref="InfoDeckDeck"/> file.
    /// </summary>
    public class InfoDeckEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x04;

        /// <summary>
        /// Gets or sets the Text page.
        /// </summary>
        public List<string> Text { get; set; } = [];
    }
}
