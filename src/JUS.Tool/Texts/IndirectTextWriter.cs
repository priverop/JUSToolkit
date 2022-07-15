using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JUS.Tool.Texts
{
    public class JusIndirectText
    {
        public Dictionary<string, int> TextOffsets { get; set; }

        public List<string> Strings { get; set; }

        public int CurrentOffset { get; set; }

        public JusIndirectText(int startingOffset)
        {
            TextOffsets = new Dictionary<string, int>();
            Strings = new List<string>();
            CurrentOffset = startingOffset;
        }
    }
}
