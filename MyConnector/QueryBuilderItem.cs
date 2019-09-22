using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector
{
    public class QueryBuilderItem
    {
        public string FieldName { get; set; }
        public object Value { get; set; }
        public bool AddSlash { get; set; }
        public bool RemoveSingleQuotes { get; set; }

        public QueryBuilderItem()
        {

        }
        public QueryBuilderItem(string fieldName, object value, bool addSlash = false)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            AddSlash = addSlash;
        }

    }
}
