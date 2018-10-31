namespace JUSToolkit.Converters.Bin
{
    using System;
    using Yarhl.Media.Text;
    using Yarhl.FileFormat;
    using JUSToolkit.Formats;
    using System.Collections.Generic;

    public class Bin2Po : IConverter<Bin, Po>
    {
        public Po Convert(Bin source)
        {
            Po poExport = new Po
            {
                Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es")
                {
                    LanguageTeam = "TranScene",
                }
            };

            int i = 0;
            foreach (KeyValuePair<string, int> entry in source.Text)
            {
                string sentence = entry.Key;
                if (string.IsNullOrEmpty(sentence))
                    sentence = "<!empty>";
                poExport.Add(new PoEntry(sentence) { Context = i.ToString() });
                i++;
            }

            return poExport;
        }
    }
}
