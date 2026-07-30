using System.Collections.Generic;

namespace JUS.Tool.Texts.Formats
{
    /// <summary>
    /// Single entry in a Tutorial file.
    /// </summary>
    public class TutorialEntry
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of Unknown pointers/values.
        /// </summary>
        public List<int> Unknowns { get; set; } = [];
    }
}
