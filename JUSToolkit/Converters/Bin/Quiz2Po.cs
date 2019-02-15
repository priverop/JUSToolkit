using System;
using System.Collections.Generic;
using JUSToolkit.Formats;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.Media.Text;

namespace JUSToolkit.Converters.Bin
{
    public class Quiz2Po : IConverter<BinQuiz, NodeContainerFormat>
    {

        private readonly Dictionary<string, string> sagas;

        public Quiz2Po()
        {
            sagas = new Dictionary<string, string>();
            SetSagas();
        }

        public NodeContainerFormat Convert(BinQuiz source)
        {

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var container = new NodeContainerFormat();

            int i = 0;
            Po poExport = new Po
            {
                Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es")
                {
                    LanguageTeam = "TranScene",
                }
            };
            foreach (KeyValuePair<string, int> entry in source.Text)
            {
                string sentence = entry.Key;
                if (string.IsNullOrEmpty(sentence))
                {
                    sentence = "<!empty>";
                }

                if (sentence.Length == 4 && sagas.ContainsKey(CleanName(sentence)))
                {
                    container.Root.Add(new Node(sentence, poExport));
                    poExport = new Po
                    {
                        Header = new PoHeader("Jump Ultimate Stars", "TranScene", "es")
                        {
                            LanguageTeam = "TranScene",
                        }
                    };
                }
                else {
                    poExport.Add(new PoEntry(sentence) { Context = i.ToString() });
                    i++;
                }

            }

            return container;
        }

        private string CleanName(string sentence)
        {
            return sentence.Remove(sentence.Length - 2);
        }

        private void SetSagas()
        {
            sagas.Add("gt", "gitama");
            sagas.Add("cb", "cobra");
            sagas.Add("is", "Is");
            sagas.Add("ig", "Ichigo 100%");
            sagas.Add("ct", "Oliver y Benji");
            sagas.Add("nk", "Ninku");
            sagas.Add("tz", "taizo mote king saga");
            sagas.Add("yo", "yugioh");
            sagas.Add("nn", "neuro");
            sagas.Add("na", "naruto");
            sagas.Add("nb", "nube, maestro del infierno");
            sagas.Add("tr", "reborn!");
            sagas.Add("to", "prince of tennis");
            sagas.Add("tl", "Tottemo! Lucky Man");
            sagas.Add("es", "eyesield 21");
            sagas.Add("rb", "rokudenashi blues");
            sagas.Add("hs", "Houshing Engi");
            sagas.Add("pj", "Pyuu to Fuku! Jaguar");
            sagas.Add("tc", "Jungle King Tar-chan");
            sagas.Add("rk", "Rurouni Kenshin");
            sagas.Add("dn", "death note");
            sagas.Add("yh", "yuyu");
            sagas.Add("bb", "bobobo");
            sagas.Add("bc", "black cat");
            sagas.Add("bl", "bleach");
            sagas.Add("dg", "d. gray man");
            sagas.Add("db", "dragon ball");
            sagas.Add("hk", "Hokuto no Ken");
            sagas.Add("hh", "hunter x hunter");
            sagas.Add("jj", "jojo bizarre");
            sagas.Add("dt", "Komas ");
            sagas.Add("ds", "Dr.Slump ");
            sagas.Add("oj", "Sakigake!! Otokujuku");
            sagas.Add("bu", "Busou Renkin");
            sagas.Add("kk", "kochikame");
            sagas.Add("mo", "Midori no Makibaoh");
            sagas.Add("kn", "Kinnikuman");
            sagas.Add("sk", "shaman king");
            sagas.Add("ss", "caballeros del zodiaco");
            sagas.Add("mr", "Muhyo to Rouji");
            sagas.Add("sd", "Slam Dunk");
            sagas.Add("op", "one piece ");
        }
    }
}
