// Copyright (c) 2021 Darkc0m

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using JUS.Tool.Texts.Formats;
using JUS.Tool.Utils;
using Yarhl.FileFormat;
using Yarhl.Media.Text;

namespace JUS.Tool.Texts.Converters
{
    /// <summary>
    /// Converts between Piece format and Po.
    /// </summary>
    public class Piece2Po :
        IConverter<Piece, Po>,
        IConverter<Po, Piece>
    {
        /// <summary>
        /// Converts Piece format to Po.
        /// </summary>
        /// <param name="piece">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public Po Convert(Piece piece)
        {
            Po po = JusText.GenerateJusPo();

            int i = 0;
            foreach (PieceEntry entry in piece.Entries) {
                po.Add(new PoEntry(entry.Title) {
                    Context = $"{i++}",
                    ExtractedComments = $"{entry.Unk1}-{entry.Id}",
                });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Authors)) { Context = $"{i++}" });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Info)) { Context = $"{i++}" });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Page1)) { Context = $"{i++}" });
                po.Add(new PoEntry(JusText.MergeStrings(entry.Page2)) { Context = $"{i++}" });
            }

            return po;
        }

        /// <summary>
        /// Converts Po to Piece format.
        /// </summary>
        /// <notes>
        /// Each Manga has 5 entries:
        ///       Title - 19 chars.
        ///       Authors.
        ///       Dates.
        ///       Description Page 1.
        ///       Description Page 2.
        /// </notes>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        public Piece Convert(Po po)
        {
            var piece = new Piece();
            PieceEntry entry;
            string[] metadata;

            piece.Count = po.Entries.Count / 5;

            for (int i = 0; i < piece.Count; i++) {
                entry = new PieceEntry();
                int index = i * 5;
                entry.Title = po.Entries[index].Text;

                foreach (string s in JusText.SplitStringToList(CheckLines(po.Entries[index + 1].Text.TrimEnd(), PieceEntry.LinesPerInfo, $"entry {index + 1}"), '\n', PieceEntry.LinesPerInfo)) {
                    string parsedText = Table.Instance.Encode(CheckLength(s, PieceEntry.MaxLineLengthInfo, $"entry {index + 1}"));
                    entry.Authors.Add(parsedText);
                }

                foreach (string s in JusText.SplitStringToList(CheckLines(po.Entries[index + 2].Text.TrimEnd(), PieceEntry.LinesPerInfo, $"entry {index + 2}"), '\n', PieceEntry.LinesPerInfo)) {
                    string parsedText = Table.Instance.Encode(CheckLength(s, PieceEntry.MaxLineLengthInfo, $"entry {index + 2}"));
                    entry.Info.Add(parsedText);
                }

                foreach (string s in JusText.SplitStringToList(CheckLines(po.Entries[index + 3].Text.TrimEnd(), PieceEntry.LinesPerPage, $"entry {index + 3}"), '\n', PieceEntry.LinesPerPage)) {
                    string parsedText = Table.Instance.Encode(CheckLength(s, new PieceEntry().MaxLineLength, $"entry {index + 3}"));
                    entry.Page1.Add(parsedText);
                }

                foreach (string s in JusText.SplitStringToList(CheckLines(po.Entries[index + 4].Text.TrimEnd(), PieceEntry.LinesPerPage, $"entry {index + 4}"), '\n', PieceEntry.LinesPerPage)) {
                    string parsedText = Table.Instance.Encode(CheckLength(s, new PieceEntry().MaxLineLength, $"entry {index + 4}"));
                    entry.Page2.Add(parsedText);
                }

                metadata = JusText.ParseMetadata(po.Entries[i * 5].ExtractedComments);
                entry.Unk1 = short.Parse(metadata[0]);
                entry.Id = short.Parse(metadata[1]);

                piece.Entries.Add(entry);
            }

            return piece;
        }

        private static string CheckLength(string input, int maxLength, string context)
        {
            if (input.Length > maxLength) {
                Logger.DisplayErrorMaxLength(maxLength, $"{context} - \"{input}\"");
                return input[..maxLength];
            }

            return input;
        }

        private static string CheckLines(string input, int maxLines, string context)
        {
            string[] lines = input.Split('\n');
            if (lines.Length > maxLines) {
                Logger.DisplayErrorMaxLines(maxLines, $"{context} - \"{input}\"");
                return string.Join("\n", lines[..maxLines]);
            }

            return input;
        }
    }
}
