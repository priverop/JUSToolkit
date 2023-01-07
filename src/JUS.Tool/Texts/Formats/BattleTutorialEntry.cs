using System.Collections.Generic;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a BattleTutorial file.
    /// </summary>
    public class BattleTutorialEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BattleTutorialEntry"/> class.
        /// </summary>
        public BattleTutorialEntry()
        {
            Unknowns = new List<int>();
        }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Pointer (which is the Description.Length * 2 - 1 as it's shift jis).
        /// </summary>
        public int Pointer { get; set; }

        /// <summary>
        /// Gets or sets the list of Unknown pointers/values.
        /// </summary>
        public List<int> Unknowns { get; set; }
    }
}
