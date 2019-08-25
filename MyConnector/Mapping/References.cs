using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector.Mapping
{
    public class References
    {
        public string ForeignTableName { get; set; }
        public string ForeignFieldName { get; set; }
        public string FieldName { get; set; }
        public string KeyName { get; set; }

        public References(string foreignTableName, string foreignFieldName, string fieldName, string keyName)
        {
            ForeignTableName = foreignTableName ?? throw new ArgumentNullException(nameof(foreignTableName));
            ForeignFieldName = foreignFieldName ?? throw new ArgumentNullException(nameof(foreignFieldName));
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            KeyName = keyName ?? throw new ArgumentNullException(nameof(keyName));
        }
    }
}
