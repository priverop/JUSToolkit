using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts
{
    /// <summary>
    /// Class with utilities to work with Jus text files.
    /// </summary>
    public static class JusText
    {
        /// <summary>
        /// Encoding used in Jus text files.
        /// </summary>
        public static readonly Encoding JusEncoding = Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// Generates a new Po for a Jus text file.
        /// </summary>
        /// <returns>Generated Jus <see cref="Po"/>.</returns>
        public static Po GenerateJusPo()
        {
            return new Po(new PoHeader("Jump Ultimate Stars", "tradusquare@gmail.es", "es"));
        }

        /// <summary>
        /// Split string separated by a key character into a list of strings.
        /// </summary>
        /// <param name="text">String to split.</param>
        /// <param name="separator">Key to find in order to split.</param>
        /// <param name="size">Maximum list length.</param>
        /// <returns>List of strings.</returns>
        public static List<string> SplitStringToList(string text, char separator, int size)
        {
            List<string> splitText = text.Split(separator).ToList();

            while (splitText.Count < size) {
                splitText.Add(string.Empty);
            }

            return splitText;
        }

        /// <summary>
        /// Split string into non empty entries.
        /// </summary>
        /// <param name="extractedComments">String to split.</param>
        /// <returns>Array of strings.</returns>
        public static string[] ParseMetadata(string extractedComments)
        {
            return extractedComments.Split('-', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Returns a string from current indirect stream position.
        /// </summary>
        /// <param name="reader">Reader to read from.</param>
        /// <returns><see cref="string"/> at indirect position.</returns>
        public static string ReadIndirectString(DataReader reader)
        {
            reader.Stream.PushToPosition(reader.Stream.Position + reader.ReadInt32());
            string text = reader.ReadString();
            reader.Stream.PopPosition();

            return text;
        }

        /// <summary>
        /// Writes relative pointers.
        /// </summary>
        /// <param name="text">String to write.</param>
        /// <param name="writer">Writer.</param>
        /// <param name="jit"><see cref="IndirectTextWriter.Strings"/> object to store texts and offsets (pointers).</param>
        public static void WriteStringPointer(string text, DataWriter writer, IndirectTextWriter jit)
        {
            if (jit.TextOffsets.ContainsKey(text)) {
                writer.Write(jit.TextOffsets[text] - (int)writer.Stream.Position);
            } else {
                jit.TextOffsets.Add(text, jit.CurrentOffset);
                writer.Write(jit.CurrentOffset - (int)writer.Stream.Position);
                jit.CurrentOffset += writer.DefaultEncoding.GetByteCount(text) + 1;
                jit.Strings.Add(text);
            }
        }

        /// <summary>
        /// Writes all the strings inside a <see cref="IndirectTextWriter.Strings"/>.
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="jit">JusIndirectText to get the strings from.</param>
        public static void WriteAllStrings(DataWriter writer, IndirectTextWriter jit)
        {
            foreach (string s in jit.Strings) {
                writer.Write(s);
            }
        }

        /// <summary>
        /// Merges multiple strings adding newlines between them.
        /// </summary>
        /// <param name="strings">List of strings to merge.</param>
        /// <returns>Merged string.</returns>
        public static string MergeStrings(List<string> strings)
        {
            var merged = new StringBuilder();
            foreach (string s in strings) {
                merged.Append($"{s}\n");
            }

            return merged.ToString()[..^1];
        }
    }
}
