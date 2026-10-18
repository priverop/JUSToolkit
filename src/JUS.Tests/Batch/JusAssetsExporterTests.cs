using JUS.Tool.BatchConverters;
using SceneGate.Ekona.Containers.Rom;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.TestFramework.Verify.Converters;

namespace JUS.Tests.Batch;

[TestFixture]
public class JusAssetsExporterTests
{
    [Test]
    public Task VerifyExportedContainer()
    {
        TestDataBase.IgnoreIfFileDoesNotExist(TestDataBase.SoftwareNitroRomPath);

        using NitroRom root = TestDataBase.ReadSoftware();
        NodeContainerFormat actual = new JusAssetsExporter("es").Convert(root);

        return Verifier.Verify(actual)
            .UseDirectory(TestDataBase.VerifyResourcesPath)
            .AddExtraSettings(settings => {
                settings.Converters.Add(new NodeVerifyJsonConverter(depth: 3, maxChildren: 10, serializeFormat: false));
            });
    }
}
