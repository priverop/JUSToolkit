namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a Demo file.
    /// </summary>
    public class DemoEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x14;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the first description line.
        /// </summary>
        public string Desc1 { get; set; }

        /// <summary>
        /// Gets or sets the second description line.
        /// </summary>
        public string Desc2 { get; set; }

        /// <summary>
        /// Gets or sets the third description line.
        /// </summary>
        public string Desc3 { get; set; }

        /// <summary>
        /// Gets or sets the demo Id.
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public byte Icon { get; set; }
    }
}
