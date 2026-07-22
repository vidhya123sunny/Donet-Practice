// Exercise 1: Mock Mail Sender

using NUnit.Framework;
using Moq;

public interface IMailSender
{
    bool SendMail(string to,string message);
}

public class CustomerComm
{
    private readonly IMailSender mailSender;

    public CustomerComm(IMailSender mailSender)
    {
        this.mailSender = mailSender;
    }

    public bool SendMailToCustomer()
    {
        return mailSender.SendMail(
            "cust123@abc.com",
            "Some Message");
    }
}

[TestFixture]
public class CustomerCommTests
{
    [Test]
    public void SendMail_Test()
    {
        var mockMail =
            new Mock<IMailSender>();

        mockMail
            .Setup(x =>
                x.SendMail(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .Returns(true);

        var customer =
            new CustomerComm(
                mockMail.Object);

        bool result =
            customer.SendMailToCustomer();

        Assert.IsTrue(result);
    }
}