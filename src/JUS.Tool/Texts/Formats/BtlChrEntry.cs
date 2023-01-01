using System.Collections.Generic;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a BtlChr file.
    /// </summary>
    public class BtlChrEntry
    {
        /// <summary>
        /// Entry size in bytes.
        /// </summary>
        public static readonly int EntrySize = 0xD4;

        /// <summary>
        /// Number of abilities per entry.
        /// </summary>
        public static readonly int NumAbilities = 10;

        /// <summary>
        /// Number of interactions per entry.
        /// </summary>
        public static readonly int NumInteractions = 3;

        /// <summary>
        /// String used to represent an empty ability.
        /// </summary>
        public static readonly string EmptyAbility = "◇";

        /// <summary>
        /// Initializes a new instance of the <see cref="BtlChrEntry"/> class.
        /// </summary>
        public BtlChrEntry()
        {
            AbilityNames = new List<string>();
            AbilityFuriganas = new List<string>();
            AbilityDescriptions = new List<string>();
            Interactions = new List<string>();
        }

        /// <summary>
        /// Gets or sets the character Name.
        /// </summary>
        public string ChrName { get; set; }

        /// <summary>
        /// Gets or sets the list of ability names.
        /// </summary>
        public List<string> AbilityNames { get; set; }

        /// <summary>
        /// Gets or sets the list of ability furiganas.
        /// </summary>
        public List<string> AbilityFuriganas { get; set; }

        /// <summary>
        /// Gets or sets the list of ability descriptions.
        /// </summary>
        public List<string> AbilityDescriptions { get; set; }

        /// <summary>
        /// Gets or sets the name of a passive ability.
        /// </summary>
        public string PassiveName { get; set; }

        /// <summary>
        /// Gets or sets the furigana of a passive ability.
        /// </summary>
        public string PassiveFurigana { get; set; }

        /// <summary>
        /// Gets or sets the first description of a passive ability.
        /// </summary>
        public string PassiveDescription1 { get; set; }

        /// <summary>
        /// Gets or sets the second description of a passive ability.
        /// </summary>
        public string PassiveDescription2 { get; set; }

        /// <summary>
        /// Gets or sets the list of interactions.
        /// </summary>
        public List<string> Interactions { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk1 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk2 { get; set; }

        /// <summary>
        /// Gets or sets the ??.
        /// </summary>
        public short Unk3 { get; set; }
    }
}
