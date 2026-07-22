// Exercise 1: Addition Test

using NUnit.Framework;

[TestFixture]
public class CalculatorTests
{
    [TestCase(10,20,30)]
    [TestCase(5,5,10)]
    public void Add_Test(int a,int b,int expected)
    {
        Assert.AreEqual(expected,a+b);
    }
}