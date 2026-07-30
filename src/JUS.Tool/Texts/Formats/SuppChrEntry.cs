using System.Collections.Generic;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a SuppChr file.
    /// </summary>
    public class SuppChrEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0x24;

        /// <summary>
        /// Number of abilities per entry.
        /// </summary>
        public static readonly int NumAbilities = 2;

        /// <summary>
        /// String used to represent an empty ability.
        /// </summary>
        public static readonly string EmptyAbility = "◇";

        /// <summary>
        /// Gets or sets the name of the character which has the ability.
        /// </summary>
        public string ChrName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of abilities.
        /// </summary>
        public List<string> Abilities { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of ability descriptions.
        /// </summary>
        public List<string> Descriptions { get; set; } = [];
    }
}
