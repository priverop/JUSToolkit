using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Format for Ability.bin file.
    /// </summary>
    public class Ability : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ability"/> class.
        /// </summary>
        public Ability()
        {
            Entries = new List<AbilityEntry>();
        }

        /// <summary>
        /// Gets or sets the number of entries in <see cref="AbilityEntry"/>.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="AbilityEntry" />.
        /// </summary>
        public List<AbilityEntry> Entries { get; set; }
    }
}
