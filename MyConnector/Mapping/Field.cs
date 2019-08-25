using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector.Mapping
{
    public class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool AllowNull { get; set; }
        public string Key { get; set; }
        public string Default { get; set; }
        public string Extra { get; set; }
        public string After { get; set; }
        public Field()
        {

        }

        public Field(string name, string type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public FieldAction Compare(Field field)
        {
            FieldAction fieldAction = new FieldAction();
            fieldAction.Field = field;
            fieldAction.Action = ActionType.None;


            if (field.Name.Trim() != Name.Trim())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Name", Action = ActionType.Update, Value = field.Name });
                fieldAction.Action = ActionType.Update;
            }

            if (field.Type.Trim() != Type.Trim())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Type", Action = ActionType.Update, Value = field.Type });
                fieldAction.Action = ActionType.Update;
            }

            if (field.AllowNull != AllowNull)
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Null", Action = ActionType.Update, Value = field.AllowNull });
                fieldAction.Action = ActionType.Update;
            }

            if (field.Key.Trim() != Key.Trim())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Key", Action = ActionType.Update, Value = field.Key });
                fieldAction.Action = ActionType.Update;
            }

            if (field.Default.Trim() != Default.Trim())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Default", Action = ActionType.Update, Value = field.Default });
                fieldAction.Action = ActionType.Update;
            }

            if(field.After.Trim() != After.Trim())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "After", Action = ActionType.Update, Value = field.Default });
                fieldAction.Action = ActionType.Update;
            }

            return fieldAction;
        }
    }
}
