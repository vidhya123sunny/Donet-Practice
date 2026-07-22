// Exercise 8: Exception Test

using NUnit.Framework;
using System;

[TestFixture]
public class UserTests
{
    [Test]
    public void Exception_Test()
    {
        Assert.Throws<FormatException>(
            () =>
            {
                throw new FormatException();
            });
    }
}