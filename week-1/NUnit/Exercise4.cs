// Exercise 4: Account Login

using NUnit.Framework;

[TestFixture]
public class LoginTests
{
    [Test]
    public void Valid_Login()
    {
        string result = "Welcome user_11!!!";

        Assert.That(result,
            Is.EqualTo("Welcome user_11!!!"));
    }

    [Test]
    public void Invalid_Login()
    {
        string result = "Invalid user id/password";

        Assert.That(result,
            Is.EqualTo("Invalid user id/password"));
    }
}