namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a JQuiz file.
    /// </summary>
    public class JQuizEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x28; // 40bytes

        /// <summary>
        /// Gets or sets the Manga ID.
        /// </summary>
        public byte MangaID { get; set; }

        /// <summary>
        /// Gets or sets the first unknown byte.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the second unkown byte.
        /// </summary>
        public short Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        public string Photo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the questions array.
        /// </summary>
        public string[] Questions { get; set; } = new string[4];

        /// <summary>
        /// Gets or sets the answers array.
        /// </summary>
        public string[] Answers { get; set; } = new string[4];
    }
}
