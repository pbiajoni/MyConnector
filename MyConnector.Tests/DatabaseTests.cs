using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyConnector.Mapping;

namespace MyConnector.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void TestGetTable()
        {
            Database database = new Database("database");
            Table table = new Table("table");
            table.Fields.Add(new Field("field", "varchar(100)"));
            database.Tables.Add(table);

            Assert.AreEqual(table, database.GetTable("table"));
        }
    }
}
