// Exercise 9: Ignore Test

using NUnit.Framework;

[TestFixture]
public class SampleTests
{
    [Test]
    [Ignore("Demo Ignore")]
    public void Ignore_Test()
    {
        Assert.Fail();
    }
}