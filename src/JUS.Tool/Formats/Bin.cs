using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Formats
{
    public class Bin : IFormat
    {
        public Dictionary<string, int> Text { get; set; } // String - Pointer

        public Bin(){
            Text = new Dictionary<string, int>();
        }
    }
}
