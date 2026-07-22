// Exercise 7: Leap Year

using NUnit.Framework;

[TestFixture]
public class LeapYearTests
{
    [TestCase(2024,true)]
    [TestCase(2023,false)]
    public void LeapYear_Test(
        int year,
        bool expected)
    {
        Assert.AreEqual(
            expected,
            DateTime.IsLeapYear(year));
    }
}