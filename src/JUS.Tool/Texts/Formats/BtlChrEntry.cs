using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Character Name.
        /// </summary>
        public string ChrName { get; set; }

        /// <summary>
        /// List of ability names.
        /// </summary>
        public List<string> AbilityNames { get; set; }

        /// <summary>
        /// List of ability furiganas.
        /// </summary>
        public List<string> AbilityFuriganas { get; set; }

        /// <summary>
        /// List of ability descriptions.
        /// </summary>
        public List<string> AbilityDescriptions { get; set; }

        /// <summary>
        /// Name of a passive ability.
        /// </summary>
        public string PassiveName { get; set; }

        /// <summary>
        /// Furigana of a passive ability.
        /// </summary>
        public string PassiveFurigana { get; set; }

        /// <summary>
        /// First description of a passive ability.
        /// </summary>
        public string PassiveDescription1 { get; set; }

        /// <summary>
        /// Second description of a passive ability.
        /// </summary>
        public string PassiveDescription2 { get; set; }

        /// <summary>
        /// List of interactions.
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
