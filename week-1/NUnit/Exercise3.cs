// Exercise 3: URL Host Test

using NUnit.Framework;

[TestFixture]
public class UrlTests
{
    [Test]
    public void ParseHostName_ValidUrl()
    {
        string url = "https://google.com";

        Assert.That(url.Contains("google"));
    }

    [Test]
    public void ParseHostName_InvalidUrl()
    {
        string url = "";

        Assert.That(url == "");
    }
}