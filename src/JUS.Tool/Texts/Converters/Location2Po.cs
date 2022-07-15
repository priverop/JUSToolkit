using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUS.Tool.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUS.Tool.Texts.Converters
{
    public class Location2Po :
        IConverter<Location, Po>,
        IConverter<Po, Location>
    {
        public Po Convert(Location location)
        {
            var po = JusText.GenerateJusPo();

            int i = 0;
            foreach (LocationEntry entry in location.Entries) {
                po.Add(new PoEntry(entry.Name) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Unk2}",
                });
            }

            return po;
        }

        public Location Convert(Po po)
        {
            var location = new Location();
            LocationEntry entry;
            string[] metadata;

            location.Count = po.Entries.Count;

            for (int i = 0; i < location.Count; i++) {
                entry = new LocationEntry();
                entry.Name = po.Entries[i].Text;

                metadata = JusText.ParseMetadata(po.Entries[i].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Unk2 = short.Parse(metadata[1]);

                location.Entries.Add(entry);
            }

            return location;
        }
    }
}
