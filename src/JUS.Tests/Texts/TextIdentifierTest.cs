using JUSToolkit.Utils;
using NUnit.Framework;

namespace JUS.Tests.Texts
{
    public class TextIdentifierTest
    {
        [Test]
        public void BinDictionaryTest()
        {
            Assert.AreEqual("Ability", TextIdentifier.GetTextFormat("ability_t.bin"));
            Assert.AreEqual("Tutorial", TextIdentifier.GetTextFormat("tutorial.bin"));
        }

        [Test]
        public void BinInfoDeckTest()
        {
            Assert.AreEqual("InfoDeck", TextIdentifier.GetTextFormat("bin-info-jump.bin"));
            Assert.AreEqual("InfoDeck", TextIdentifier.GetTextFormat("bin-deck-bb.bin"));
        }
    }
}
