// Exercise 2: Mock Files

using NUnit.Framework;
using Moq;
using System.Collections.Generic;

public interface IDirectoryExplorer
{
    ICollection<string> GetFiles(string path);
}

[TestFixture]
public class DirectoryTests
{
    [Test]
    public void GetFiles_Test()
    {
        var mockDir =
            new Mock<IDirectoryExplorer>();

        mockDir
            .Setup(x =>
                x.GetFiles(It.IsAny<string>()))
            .Returns(
                new List<string>
                {
                    "file.txt",
                    "file2.txt"
                });

        var files =
            mockDir.Object.GetFiles("C:");

        Assert.IsNotNull(files);
        Assert.AreEqual(2, files.Count);
        CollectionAssert.Contains(
            files,
            "file.txt");
    }
}