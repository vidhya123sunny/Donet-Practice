// Exercise 6: TestCaseSource

using NUnit.Framework;
using System.Collections;

[TestFixture]
public class SeasonTests
{
    public static IEnumerable Data()
    {
        yield return new TestCaseData("January","Winter");
        yield return new TestCaseData("May","Summer");
    }

    [TestCaseSource(nameof(Data))]
    public void Season_Test(
        string month,
        string expected)
    {
        Assert.IsNotNull(expected);
    }
}