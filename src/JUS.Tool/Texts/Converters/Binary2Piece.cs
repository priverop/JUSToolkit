using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUS.Tool.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUS.Tool.Texts.Converters
{
    public class Binary2Piece :
        IConverter<BinaryFormat, Piece>,
        IConverter<Piece, BinaryFormat>
    {
        private DataReader reader;
        private DataWriter writer;

        public Piece Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var piece = new Piece();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            piece.Count = reader.ReadInt32();
            for (int i = 0; i < piece.Count; i++) {
                piece.Entries.Add(ReadEntry());
            }

            return piece;
        }

        public BinaryFormat Convert(Piece piece)
        {
            var bin = new BinaryFormat();
            writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new JusIndirectText((PieceEntry.EntrySize * piece.Count) + 0x04);

            writer.Write(piece.Count);

            foreach (PieceEntry entry in piece.Entries) {
                JusText.WriteStringPointer(entry.Title, writer, jit);
                foreach (string s in entry.Authors) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Info) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Page1) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                foreach (string s in entry.Page2) {
                    JusText.WriteStringPointer(s, writer, jit);
                }

                writer.Write(entry.Unk1);
                writer.Write(entry.Id);
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        private PieceEntry ReadEntry()
        {
            var entry = new PieceEntry();

            entry.Title = JusText.ReadIndirectString(reader);
            for (int i = 0; i < PieceEntry.NumAuthors; i++) {
                entry.Authors.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < PieceEntry.NumInfo; i++) {
                entry.Info.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < PieceEntry.LinesPerPage; i++) {
                entry.Page1.Add(JusText.ReadIndirectString(reader));
            }

            for (int i = 0; i < PieceEntry.LinesPerPage; i++) {
                entry.Page2.Add(JusText.ReadIndirectString(reader));
            }

            entry.Unk1 = reader.ReadInt16();
            entry.Id = reader.ReadInt16();

            return entry;
        }
    }
}
