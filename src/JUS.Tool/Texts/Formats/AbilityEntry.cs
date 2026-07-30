namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a Ability file.
    /// </summary>
    public class AbilityEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0xC; // 12

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first description line.
        /// </summary>
        public string Description1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the second description line.
        /// </summary>
        public string Description2 { get; set; } = string.Empty;
    }
}
