using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

            Engine = "InnoDB";
            DefaultCharset = "utf8";
            Collate = "utf8_unicode_ci";
        }

        public string GetCreateTable(Table table)
        {
            string cmd = "CREATE TABLE `" + table.Name + "` (";

            foreach (Field field in table.Fields)
            {
                cmd += "`" + field.Name + "` " + field.Type + " ";

                if (!field.AllowNull)
                {
                    if (!string.IsNullOrEmpty(field.Extra) && field.Extra.ToUpper() == "AUTO_INCREMENT")
                    {
                        cmd += "NOT NULL " + field.Extra + ",";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(field.Default))
                        {
                            throw ExceptionManager.Exception("The default value must contain a value when not null.", table.Name, field.Name);
                        }

                        cmd += "NOT NULL DEFAULT " + field.Default + ",";
                    }
                }
                else
                {
                    cmd += string.IsNullOrEmpty(field.Default) ? "DEFAULT NULL," : "DEFAULT " + field.Default + ",";
                }

            }

            foreach (Field field in table.Fields)
            {
                if (!string.IsNullOrEmpty(field.Key) && field.Key.ToUpper() == "PRI")
                {
                    cmd += "PRIMARY KEY (`" + field.Name + "`),";
                }
            }

            foreach (Index index in table.Indexes)
            {
                cmd += (index.Unique ? "UNIQUE " : "") + "KEY `" + index.KeyName + "` (`" + index.ColumnName + "`),";
            }

            foreach (References reference in table.References)
            {
                cmd += "CONSTRAINT `" + reference.KeyName + "` FOREIGN KEY (`" + reference.FieldName + "`) " +
                    "REFERENCES `" + reference.ForeignTableName + "` (`" + reference.ForeignFieldName + "`),";
            }

            cmd = cmd.TrimEnd(',') + ") ";
            cmd += (!string.IsNullOrEmpty(table.Engine) ? "ENGINE=" + table.Engine :
                throw ExceptionManager.Exception("Engine value cannot be null", table.Name, "")) + " ";
            cmd += (!string.IsNullOrEmpty(table.DefaultCharset) ? "DEFAULT CHARSET=" + table.DefaultCharset :
                throw ExceptionManager.Exception("Default Charset value cannot be null", table.Name, "")) + " ";
            cmd += (!string.IsNullOrEmpty(table.Collate) ? "COLLATE=" + table.Collate :
                throw ExceptionManager.Exception("Collate value cannot be null", table.Name, "")) + " ";

            return cmd;
        }
        public string GetCreateTable()
        {
            return GetCreateTable(this);
        }

        public void AddVarCharField(string name, int size, string defaultValue = "", string after = "")
        {
            if (Fields.Any(x => x.Name == name))
            {
                throw new Exception("the field name " + name + " already exists");
            }

            Fields.Add(new Field()
            {
                Name = name,
                AllowNull = true,
                Default = defaultValue,
                Type = "VARCHAR(" + size.ToString() + ")",
                After = after
            });
        }
    }
}
