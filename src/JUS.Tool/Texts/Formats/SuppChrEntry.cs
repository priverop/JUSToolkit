using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUSToolkit.Texts.Formats
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
        /// Initializes a new instance of the <see cref="SuppChrEntry"/> class.
        /// </summary>
        public SuppChrEntry()
        {
            Abilities = new List<string>();
            Descriptions = new List<string>();
        }

        /// <summary>
        /// Name of the character which has the ability.
        /// </summary>
        public string chrName { get; set; }

        /// <summary>
        /// List of abilities.
        /// </summary>
        public List<string> Abilities { get; set; }

        /// <summary>
        /// List of ability descriptions.
        /// </summary>
        public List<string> Descriptions { get; set; }
    }
}
