using JUS.Tool.Texts.Converters;
using JUS.Tool.Utils;
using NUnit.Framework;

namespace JUS.Tests.Texts
{
    public class TextIdentifierTest
    {
        [Test]
        public void BinDictionaryTest()
        {
            Assert.That(TextIdentifier.GetTextFormat("ability_t.bin")[0], Is.EqualTo(typeof(Binary2Ability)));
            Assert.That(TextIdentifier.GetTextFormat("tutorial.bin")[1], Is.EqualTo(typeof(Tutorial2Po)));
        }

        [Test]
        public void BinInfoDeckTest()
        {
            Assert.That(TextIdentifier.GetTextFormat("bin-info-jump.bin")[0], Is.EqualTo(typeof(Binary2InfoDeckInfo)));
            Assert.That(TextIdentifier.GetTextFormat("bin-deck-bb.bin")[1], Is.EqualTo(typeof(InfoDeckDeck2Po)));
        }
    }
}
