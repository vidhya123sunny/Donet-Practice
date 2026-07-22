// Exercise 3: Mock Database

using NUnit.Framework;
using Moq;

public interface IPlayerMapper
{
    bool IsPlayerNameExistsInDb(
        string name);

    void AddNewPlayerIntoDb(
        string name);
}

[TestFixture]
public class PlayerTests
{
    [Test]
    public void RegisterPlayer_Test()
    {
        var mockDb =
            new Mock<IPlayerMapper>();

        mockDb
            .Setup(x =>
                x.IsPlayerNameExistsInDb(
                    It.IsAny<string>()))
            .Returns(false);

        bool exists =
            mockDb.Object
                  .IsPlayerNameExistsInDb(
                      "Virat");

        Assert.IsFalse(exists);
    }
}