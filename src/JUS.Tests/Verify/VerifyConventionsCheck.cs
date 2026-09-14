namespace JUS.Tests.Verify;

[TestFixture]
public class VerifyConventionsCheck
{
    /// <summary>
    /// Ensures the git repository is set up according to the
    /// git conventions of Verify, so it can perform diffs properly.
    /// </summary>
    [Test]
    public Task Run() => VerifyChecks.Run();
}
