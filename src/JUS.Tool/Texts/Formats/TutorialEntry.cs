using System.Collections.Generic;

namespace JUSToolkit.Texts.Formats
{
    /// <summary>
    /// Single entry in a Tutorial file.
    /// </summary>
    public class TutorialEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TutorialEntry"/> class.
        /// </summary>
        public TutorialEntry()
        {
            Unknowns = new List<int>();
        }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the list of Unknown pointers/values.
        /// </summary>
        public List<int> Unknowns { get; set; }
    }
}
