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
        public OnDelete OnDeleteAction { get; set; }
        public OnUpdate OnUpdateAction { get; set; }

        public References(string foreignTableName, string foreignFieldName, string fieldName, string keyName,
            OnDelete onDeleteAction, OnUpdate onUpdateAction) : this(foreignTableName, foreignFieldName, fieldName, keyName)
        {
            OnDeleteAction = onDeleteAction;
            OnUpdateAction = onUpdateAction;
        }

        public References(string foreignTableName, string foreignFieldName, string fieldName, string keyName)
        {
            ForeignTableName = foreignTableName ?? throw new ArgumentNullException(nameof(foreignTableName));
            ForeignFieldName = foreignFieldName ?? throw new ArgumentNullException(nameof(foreignFieldName));
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            KeyName = keyName ?? throw new ArgumentNullException(nameof(keyName));
            OnDeleteAction = OnDelete.Restrict;
            OnUpdateAction = OnUpdate.Restrict;
        }

        public References(string foreignTableName, string fieldName, string keyName)
        {
            ForeignTableName = foreignTableName ?? throw new ArgumentNullException(nameof(foreignTableName));
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            KeyName = keyName ?? throw new ArgumentNullException(nameof(keyName));
            ForeignFieldName = "id";
        }

        public References(string foreignTableName, string fieldName, string keyName, 
            OnDelete onDeleteAction, OnUpdate onUpdateAction) : this(foreignTableName, fieldName, keyName)
        {
            OnDeleteAction = onDeleteAction;
            OnUpdateAction = onUpdateAction;
            ForeignFieldName = "id";
        }

        public override bool Equals(object obj)
        {
            References cref = (References)obj;

            if(cref.FieldName != FieldName)
            {
                return true;
            }

            if(cref.ForeignFieldName != ForeignFieldName)
            {
                return true;
            }

            if(cref.ForeignTableName != ForeignTableName)
            {
                return true;
            }

            if(cref.KeyName != KeyName)
            {
                return true;
            }

            if(cref.OnDeleteAction != OnDeleteAction)
            {
                return true;
            }

            if(cref.OnUpdateAction != OnUpdateAction)
            {
                return true;
            }

            return false;
        }
    }
}
