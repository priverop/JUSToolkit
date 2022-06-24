﻿using System.Collections.Generic;
using Yarhl.FileFormat;

namespace JUSToolkit.Formats
{
    class BinInfoTitle : IFormat
    {
        public List<string> Text;
        public List<int> Pointers;


        public BinInfoTitle()
        {
            Text = new List<string>();
            Pointers = new List<int>();
        }
    }
}
