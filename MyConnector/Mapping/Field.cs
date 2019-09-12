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
            Grant();
        }

        public Field(string name, string type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));

            Grant();
        }

        void Grant()
        {
            Default = "";
        }

        public FieldAction Compare(Field field)
        {
            Console.WriteLine("Comparing " + field.Name);
            FieldAction fieldAction = new FieldAction();
            fieldAction.Field = field;
            fieldAction.Action = ActionType.None;

            if (field.Name.Trim().ToUpper() != Name.Trim().ToUpper())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Name", Action = ActionType.Update, Value = field.Name });
                fieldAction.Action = ActionType.Update;
                Console.WriteLine(field.Name + " was changed - param Name - " + field.Name + " to " + Name);
            }

            if (field.Type.Trim().ToUpper() != Type.Trim().ToUpper())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Type", Action = ActionType.Update, Value = field.Type });
                fieldAction.Action = ActionType.Update;
                Console.WriteLine(field.Name + " was changed - param Type- " + field.Type + " to " + Type);
            }

            if (field.AllowNull != AllowNull)
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Null", Action = ActionType.Update, Value = field.AllowNull });
                fieldAction.Action = ActionType.Update;
                Console.WriteLine(field.Name + " was changed - param AllowNull - " + field.AllowNull.ToString() + " to " + AllowNull.ToString());
            }

            //if ((field.Key != "UNI" && !string.IsNullOrEmpty(Key)) && (field.Key.Trim().ToUpper() != Key.Trim().ToUpper()))
            //{
            //    fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Key", Action = ActionType.Update, Value = field.Key });
            //    fieldAction.Action = ActionType.Update;
            //    Console.WriteLine(field.Name + " was changed - param Key");
            //}

            if (field.Default.Trim().ToUpper() != Default.Trim().ToUpper())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "Default", Action = ActionType.Update, Value = field.Default });
                fieldAction.Action = ActionType.Update;
                Console.WriteLine(field.Name + " was changed - param Default - " + field.Default + " to " + Default);
            }

            if (!string.IsNullOrEmpty(field.After) && field.After.Trim() != After.Trim().ToUpper())
            {
                fieldAction.PropertyActions.Add(new PropertyAction() { Name = "After", Action = ActionType.Update, Value = field.Default });
                fieldAction.Action = ActionType.Update;
                Console.WriteLine(field.Name + " was changed - param After - " + field.After + " to " + After);
            }

            return fieldAction;
        }
    }
}
