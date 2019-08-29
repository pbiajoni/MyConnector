using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyConnector.Mapping;

namespace MyConnector.Tests
{
    [TestClass]
    public class FieldsTests
    {
        [TestMethod]
        public void TestGetVarCharFieldLength()
        {
            Table table = new Table("table");
            table.Fields.Add(new Field("field", "varchar(100)"));
            int len = table.GetFieldLength("field");

            Assert.AreEqual(100, len);
        }
    }
}
