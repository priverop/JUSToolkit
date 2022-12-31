using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JUSToolkit.Texts.Converters;
using JUSToolkit.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    public class AbilityFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/Ability/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void AbilityTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Ability
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Ability = new Binary2Ability();
                    Ability expectedAbility = null;
                    try {
                        expectedAbility = binary2Ability.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Ability with {node.Path}\n{ex}");
                    }

                    // Ability -> Po
                    var ability2Po = new Ability2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = ability2Po.Convert(expectedAbility);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Ability -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Ability
                    Ability actualAbility = null;
                    try {
                        actualAbility = ability2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Ability with {node.Path}\n{ex}");
                    }

                    // Ability -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Ability.Convert(actualAbility);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Stage -> Ability with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Ability is not identical: {node.Path}");
                }
            }
        }
    }
}
