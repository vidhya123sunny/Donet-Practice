// Exercise 2: Parameterized Tests

using NUnit.Framework;

[TestFixture]
public class ParameterizedCalculatorTests
{
    [TestCase(10,5,5)]
    [TestCase(20,10,10)]
    public void Subtract_Test(int a,int b,int expected)
    {
        Assert.AreEqual(expected,a-b);
    }

    [TestCase(5,2,10)]
    [TestCase(10,10,100)]
    public void Multiply_Test(int a,int b,int expected)
    {
        Assert.AreEqual(expected,a*b);
    }

    [TestCase(10,2,5)]
    public void Divide_Test(int a,int b,int expected)
    {
        Assert.AreEqual(expected,a/b);
    }
}
