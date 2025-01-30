using JUSToolkit.Texts.Converters;
using JUSToolkit.Utils;
using NUnit.Framework;

namespace JUS.Tests.Texts
{
    public class TextIdentifierTest
    {
        [Test]
        public void BinDictionaryTest()
        {
            Assert.AreEqual(typeof(Binary2Ability), TextIdentifier.GetTextFormat("ability_t.bin")[0]);
            Assert.AreEqual(typeof(Tutorial2Po), TextIdentifier.GetTextFormat("tutorial.bin")[1]);
        }

        [Test]
        public void BinInfoDeckTest()
        {
            Assert.AreEqual(typeof(Binary2InfoDeckInfo), TextIdentifier.GetTextFormat("bin-info-jump.bin")[0]);
            Assert.AreEqual(typeof(InfoDeck2Po), TextIdentifier.GetTextFormat("bin-deck-bb.bin")[1]);
        }
    }
}
