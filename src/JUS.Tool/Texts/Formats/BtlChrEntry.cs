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

        public BtlChrEntry()
        {
            AbilityNames = new List<string>();
            AbilityFuriganas = new List<string>();
            AbilityDescriptions = new List<string>();
            Interactions = new List<string>();
        }

        public string ChrName { get; set; }

        public List<string> AbilityNames { get; set; }

        public List<string> AbilityFuriganas { get; set; }

        public List<string> AbilityDescriptions { get; set; }

        public string PassiveName { get; set; }

        public string PassiveFurigana { get; set; }

        public string PassiveDescription1 { get; set; }

        public string PassiveDescription2 { get; set; }

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
