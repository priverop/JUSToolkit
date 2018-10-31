namespace JUSToolkit.Formats
{
    using Yarhl.FileFormat;
    using System;
    using System.Collections.Generic;
    public class Bin : Format
    {
        public Dictionary<string, int> Text { get; set; } // String - Pointer

        public Bin(){
            Text = new Dictionary<string, int>();
        }
    }
}
