// Exercise 5: Collections

using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class CollectionTests
{
    [Test]
    public void EmployeeCollection_Test()
    {
        List<int> employees =
            new List<int>{100,101,102};

        CollectionAssert.Contains(
            employees,
            100);

        CollectionAssert.AllItemsAreUnique(
            employees);
    }
}