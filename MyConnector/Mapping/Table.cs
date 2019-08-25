using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector.Mapping
{
    public class Table
    {
        public int CreateIndex { get; set; }
        public string Name { get; set; }
        public string Engine { get; set; }
        public string DefaultCharset { get; set; }
        public string Collate { get; set; }
        public List<Field> Fields { get; set; }
        public List<References> References { get; set; }
        public List<Index> Indexes { get; set; }

        public Table()
        {
            GrantLists();
        }

        public Table(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GrantLists();
        }

        public void GrantLists()
        {
            if (Indexes is null)
            {
                Indexes = new List<Index>();
            }

            if (Fields is null)
            {
                Fields = new List<Field>();
            }

            if (References is null)
            {
                References = new List<References>();
            }
        }

        public string GetCreateTable(Table table)
        {
            string cmd = "CREATE TABLE `" + table.Name + "` (";

            foreach (Field field in table.Fields)
            {
                cmd += "`" + field.Name + "` " + field.Type + " " + (!field.AllowNull ? "NOT NULL" : "") +
                    " DEFAULT " + (string.IsNullOrEmpty(field.Default) ? "NULL" : field.Default) + " " + field.Extra + ",";
            }

            foreach (Field field in table.Fields)
            {
                if (field.Key.ToUpper() == "PRI")
                {
                    cmd += "PRIMARY KEY (`" + field.Name + "`),";
                }
            }

            foreach (Index index in table.Indexes)
            {
                cmd += (index.Unique ? "UNIQUE " : "") + "KEY `" + index.KeyName + "` (`" + index.ColumnName + "`), ";
            }

            foreach (References reference in table.References)
            {
                cmd += "CONSTRAINT `" + reference.KeyName + "` FOREIGN KEY (`" + reference.FieldName + "`) " +
                    "REFERENCES `" + reference.ForeignTableName + "` (`" + reference.ForeignFieldName + "`),";
            }

            cmd = cmd.TrimEnd(',') + ") ";
            cmd += (string.IsNullOrEmpty(table.Engine) ? "ENGINE=" + table.Engine : "") + " ";
            cmd += (string.IsNullOrEmpty(table.DefaultCharset) ? "DEFAULT CHARSET=" + table.DefaultCharset : "") + " ";
            cmd += (string.IsNullOrEmpty(table.Collate) ? "COLLATE=" + table.Collate : "") + " ";

            return cmd;
        }
        public string GetCreateTable()
        {
            return GetCreateTable(this);
        }
    }
}
