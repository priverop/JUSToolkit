using JUS.Tool.BatchConverters;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

[TestFixture]
public class JusTextAssetsExporterTests
{
    [Test]
    public void ExportedContainerAreNotDisposed()
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        NodeContainerFormat actual;
        using (NitroRom testRoot = TestDataBase.ReadSoftware()) {
            actual = new JusTextAssetsExporter(createTemplate: true).Convert(testRoot);
        }

        foreach (Node exportedNode in Navigator.IterateNodes(actual.Root)) {
            Assert.That(exportedNode.Disposed, Is.False);

            if (exportedNode.Format is IBinary { Stream: DataStream dataStream }) {
                Assert.That(dataStream.Disposed, Is.False);
            }
        }
    }

    [Test]
    public Task VerifyExportedContainer()
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        using NitroRom root = TestDataBase.ReadSoftware();
        NodeContainerFormat actual = new JusTextAssetsExporter(createTemplate: false).Convert(root);

        return Verifier.Verify(actual)
            .UseDirectory(TestDataBase.VerifyTextsPath)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter());
                settings.Converters.Add(new BinaryFormatVerifyJsonConverter());
            });
    }
}
