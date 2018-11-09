namespace JUSToolkit.Formats
{
    using System;
    using System.Collections.Generic;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    public class ALAR : Format
    {
        public List<Node> Files { get; set; }

        public ALAR()
        {
            Files = new List<Node>();
        }
    }
}
