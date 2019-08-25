using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector.Mapping
{
    public class ExceptionManager
    {
        public static Exception Exception(string message, string tableName, string fieldName)
        {
            return new Exception(message + (!string.IsNullOrEmpty(tableName) ? Environment.NewLine + "Table " + tableName : "") + "" +
                (!string.IsNullOrEmpty(fieldName) ? Environment.NewLine + "Field " + fieldName : ""));
        }
    }
}
