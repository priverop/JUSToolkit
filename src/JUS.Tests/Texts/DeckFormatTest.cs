﻿using System;
using System.IO;
using JUSToolkit.Texts.Converters;
using JUSToolkit.Texts.Formats;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace JUS.Tests.Texts
{
    public class DeckFormatTest
    {
        private string resPath;

        [SetUp]
        public void Setup()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            resPath = Path.GetFullPath(programDir + "/../../../" + "Resources/Texts/Deck/");

            Assert.True(Directory.Exists(resPath), "The resources folder does not exist", resPath);
        }

        [Test]
        public void DeckTest()
        {
            foreach (string filePath in Directory.GetFiles(resPath, "*.bin", SearchOption.AllDirectories)) {
                using (var node = NodeFactory.FromFile(filePath)) {
                    // BinaryFormat -> Deck
                    var expectedBin = node.GetFormatAs<BinaryFormat>();
                    var binary2Deck = new Binary2Deck();
                    Deck expectedDeck = null;
                    try {
                        expectedDeck = binary2Deck.Convert(expectedBin);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception BinaryFormat -> Deck with {node.Path}\n{ex}");
                    }

                    // Deck -> Po
                    var deck2Po = new Deck2Po();
                    Po expectedPo = null;
                    try {
                        expectedPo = deck2Po.Convert(expectedDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Deck -> Po with {node.Path}\n{ex}");
                    }

                    // Po -> Deck
                    Deck actualDeck = null;
                    try {
                        actualDeck = deck2Po.Convert(expectedPo);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Po -> Deck with {node.Path}\n{ex}");
                    }

                    // Deck -> BinaryFormat
                    BinaryFormat actualBin = null;
                    try {
                        actualBin = binary2Deck.Convert(actualDeck);
                    } catch (Exception ex) {
                        Assert.Fail($"Exception Deck -> BinaryFormat with {node.Path}\n{ex}");
                    }

                    // Comparing Binaries
                    Assert.True(expectedBin.Stream.Compare(actualBin.Stream), $"Deck are not identical: {node.Path}");
                }
            }
        }
    }
}
