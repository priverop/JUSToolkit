// Copyright (c) 2022 Priverop

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
using System;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.IO;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between JQuiz format and BinaryFormat.
    /// </summary>
    public class Binary2JQuiz : IConverter<BinaryFormat, JQuiz>, IConverter<JQuiz, BinaryFormat>
    {
        private DataReader reader;

        /// <summary>
        /// Converts BinaryFormat to JQuiz format.
        /// </summary>
        /// <param name="source">BinaryFormat to convert.</param>
        /// <returns>Text format.</returns>
        /// <exception cref="ArgumentNullException">Source file does not exist.</exception>
        public JQuiz Convert(BinaryFormat source)
        {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            var jquiz = new JQuiz();
            reader = new DataReader(source.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            jquiz.NumQuestions = reader.ReadInt32();

            for (int i = 0; i < jquiz.NumQuestions; i++) {
                jquiz.Entries.Add(ReadEntry());
            }

            return jquiz;
        }

        /// <summary>
        /// Converts JQuiz format to BinaryFormat.
        /// </summary>
        /// <param name="jquiz">TextFormat to convert.</param>
        /// <returns>BinaryFormat.</returns>
        public BinaryFormat Convert(JQuiz jquiz)
        {
            var bin = new BinaryFormat();
            var writer = new DataWriter(bin.Stream) {
                DefaultEncoding = JusText.JusEncoding,
            };

            var jit = new IndirectTextWriter(0x04 + (jquiz.NumQuestions * JQuizEntry.EntrySize)); // debug: 120244

            writer.Write(jquiz.NumQuestions);

            // debug: First iteration -> 120399 current offset + 7 strings + 7 textsoffsets
            // debug: Second iteration -> 120509 current offset + 13 strings + 13 textsoffsets
            foreach (JQuizEntry entry in jquiz.Entries) {
                writer.Write(entry.MangaID);
                writer.Write(entry.Unknown);
                writer.Write(entry.Unknown2);
                JusText.WriteStringPointer(JusText.WriteCleanString(entry.Photo), writer, jit);
                for (int i = 0; i < entry.Questions.Length; i++) {
                    JusText.WriteStringPointer(JusText.WriteCleanString(entry.Questions[i]), writer, jit);
                }

                for (int i = 0; i < entry.Answers.Length; i++) {
                    JusText.WriteStringPointer(JusText.WriteCleanString(entry.Answers[i]), writer, jit);
                }
            }

            JusText.WriteAllStrings(writer, jit);

            return bin;
        }

        /// <summary>
        /// Reads a single <see cref="JQuizEntry"/>.
        /// </summary>
        /// <returns>The read <see cref="JQuizEntry"/>.</returns>
        private JQuizEntry ReadEntry()
        {
            var entry = new JQuizEntry {
                MangaID = reader.ReadByte(),
                Unknown = reader.ReadByte(),
                Unknown2 = reader.ReadInt16(),
                Photo = JusText.ReadIndirectString(reader),
            };

            for (int i = 0; i < entry.Questions.Length; i++) {
                entry.Questions[i] = JusText.ReadIndirectString(reader);
            }

            for (int i = 0; i < entry.Answers.Length; i++) {
                entry.Answers[i] = JusText.ReadIndirectString(reader);
            }

            return entry;
        }
    }
}
