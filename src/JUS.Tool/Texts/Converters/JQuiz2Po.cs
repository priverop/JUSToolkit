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
using System.Collections.Generic;
using JUSToolkit.Texts.Formats;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUSToolkit.Texts.Converters
{
    /// <summary>
    /// Converts between JQuiz format and NodeContainerFormat.
    /// </summary>
    public class JQuiz2Po :
        IConverter<JQuiz, NodeContainerFormat>
    {
        private readonly Dictionary<int, string> mangaIndex = new Dictionary<int, string>() {
            { 0x00, "Shonen Jump" },
            { 0x01, "Eyeshield 21" },
            { 0x02, "I's" },
            { 0x03, "Ichigo 100%" },
            { 0x04, "Tutor Hitman Reborn!" },
            { 0x05, "Oliver y Benji" },
            { 0x06, "Gintama" },
            { 0x07, "Musculman" },
            { 0x08, "Kochikame" },
            { 0x09, "Cobra" },
            { 0x0A, "Sakigake!! Otokojuku" },
            { 0x0B, "Nube, el maestro del infierno" },
            { 0x0C, "Shaman King" },
            { 0x0D, "Jungle King Tar-chan" },
            { 0x0E, "JoJo's Bizarre Adventure" },
            { 0x0F, "Slam Dunk" },
            { 0x10, "Los caballeros del Zodiaco" },
            { 0x11, "Taizo Mote King Saga" },
            { 0x12, "The Prince of Tennis" },
            { 0x13, "D. Gray-Man" },
            { 0x14, "Death Note" },
            { 0x15, "Dr. Slump" },
            { 0x16, "Tottemo! Luckyman" },
            { 0x17, "Dragon Ball" },
            { 0x18, "Naruto" },
            { 0x19, "Ninku" },
            { 0x1A, "Hunter x Hunter" },
            { 0x1B, "Pyu to Fuku! Jaguar" },
            { 0x1C, "Buso Renkin" },
            { 0x1D, "Black Cat" },
            { 0x1E, "Bleach" },
            { 0x1F, "Hoshin Engi" },
            { 0x20, "El puno de la estrella del norte" },
            { 0x21, "Bobobo" },
            { 0x22, "Nogami Neuro, el Detective Demoniaco" },
            { 0x23, "Midori no Makibao" },
            { 0x24, "Muhyo y Roji" },
            { 0x25, "Yu-Gi-Oh!" },
            { 0x26, "Yu Yu Hakusho" },
            { 0x27, "Ruroni Kenshin, el guerrero samurai" },
            { 0x28, "Rokudenashi Blues" },
            { 0x29, "One Piece" },
            { 0xFF, "Dr. Slump" },
        };

        /// <summary>
        /// Converts JQuiz format to a Container of Po.
        /// </summary>
        /// <param name="jquiz">TextFormat to convert.</param>
        /// <returns>Po format.</returns>
        public NodeContainerFormat Convert(JQuiz jquiz)
        {
            var po = JusText.GenerateJusPo();
            var container = new NodeContainerFormat();
            BinaryFormat poBin;

            int currentManga = 0xFF;
            int mangaCount = 0;
            int questionCount = 0;
            foreach (JQuizEntry entry in jquiz.Entries) {
                if (entry.MangaID != currentManga) {
                    poBin = ConvertFormat.With<Po2Binary>(po) as BinaryFormat;
                    container.Root.Add(new Node($"jquiz-{mangaCount}-{mangaIndex[currentManga]}.po", poBin));
                    po = JusText.GenerateJusPo();
                    mangaCount++;
                    currentManga = entry.MangaID;
                }

                po.Add(new PoEntry(JusText.CleanString(entry.Photo)) {
                    Context = $"Pregunta {questionCount} foto",
                    ExtractedComments = $"{entry.MangaID}-{entry.Unknown}-{entry.Unknown2}",
                });
                for (int j = 0; j < entry.Questions.Length; j++) {
                    po.Add(new PoEntry(JusText.CleanString(entry.Questions[j])) {
                        Context = $"Pregunta {questionCount} enunciado {j}",
                    });
                }

                for (int k = 0; k < entry.Answers.Length; k++) {
                    po.Add(new PoEntry(JusText.CleanString(entry.Answers[k])) {
                        Context = $"Pregunta {questionCount} respuesta {k}",
                    });
                }

                questionCount++;
            }
            poBin = ConvertFormat.With<Po2Binary>(po) as BinaryFormat;
            container.Root.Add(new Node($"jquiz-{mangaCount}-{mangaIndex[currentManga]}.po", poBin));

            return container;
        }

        /// <summary>
        /// Converts Po to JQuiz format.
        /// </summary>
        /// <param name="po">Po to convert.</param>
        /// <returns>Transformed TextFormat.</returns>
        // public JQuiz Convert(Po po)
        // {
        //     var jquiz = new JQuiz();
        //     JQuizEntry entry;
        //     List<string> description;

        //     jquiz.Count = po.Entries.Count / 2;

        //     for (int i = 0; i < jquiz.Count; i++) {
        //         entry = new JQuizEntry();

        //         entry.Title = po.Entries[i * 2].Text;

        //         string descriptionEntry = po.Entries[(i * 2) + 1].Text;
        //         if (descriptionEntry == "<!empty>") {
        //             entry.Description1 = string.Empty;
        //             entry.Description2 = string.Empty;
        //         } else {
        //             description = JusText.SplitStringToList(descriptionEntry, '\n', 2);
        //             entry.Description1 = description[0];
        //             entry.Description2 = description[1];
        //         }

        //         jquiz.Entries.Add(entry);
        //     }

        //     return jquiz;
        // }
    }
}
