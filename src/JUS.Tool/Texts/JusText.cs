using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tool.Texts
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
        /// <returns>Generated Jus <see cref="Po"/></returns>
        public static Po GenerateJusPo()
        {
            return new Po(new PoHeader("Jump Ultimate Stars", "tradusquare@gmail.es", "es"));
        }

        public static string[] SplitStringToFixedSizeArray(string text, char separator, int size)
        {
            string[] splitText = text.Split(separator, size);

            if (splitText.Length < size) {
                string[] fixedArray = new string[3];
                for (int i = 0; i < splitText.Length; i++) {
                    fixedArray[i] = splitText[i];
                }

                return fixedArray;
            }

            return splitText;
        }

        public static string[] ParseMetadata(string extractedComments)
        {
            return extractedComments.Split('-', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Returns a string from current indirect stream position.
        /// </summary>
        /// <param name="reader">Reader to read from.</param>
        /// <returns>String at indirect position.</returns>
        public static string ReadIndirectString(DataReader reader)
        {
            reader.Stream.PushToPosition(reader.Stream.Position + reader.ReadInt32());
            string text = reader.ReadString();
            reader.Stream.PopPosition();

            return text;
        }

        public static void WriteStringPointer(string text, DataWriter writer, JusIndirectText jit)
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
        /// Writes all the strings in <see cref="JusIndirectText.Strings"/>.
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="jit">JusIndirectText to get the strings from.</param>
        public static void WriteAllStrings(DataWriter writer, JusIndirectText jit)
        {
            foreach (string s in jit.Strings) {
                writer.Write(s);
            }
        }
    }
}
