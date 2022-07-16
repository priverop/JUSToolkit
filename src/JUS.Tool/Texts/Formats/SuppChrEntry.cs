using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUS.Tool.Texts.Formats
{
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

        public SuppChrEntry()
        {
            Abilities = new List<string>();
            Descriptions = new List<string>();
        }

        public string chrName { get; set; }

        public List<string> Abilities { get; set; }

        public List<string> Descriptions { get; set; }
    }
}
