﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyConnector.Mapping
{
    public class Table
    {
        public List<string> PrePopulateCommands { get; set; }
        public string DatabaseName { get; internal set; }
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

            if (PrePopulateCommands == null)
            {
                PrePopulateCommands = new List<string>();
            }

            Engine = "InnoDB";
            DefaultCharset = "utf8";
            Collate = "utf8_unicode_ci";
        }

        public int GetFieldLength(string fieldName)
        {
            return Convert.ToInt32(Utils.Between(Fields.Single(x => x.Name == fieldName).Type, "(", ")"));
        }

        bool ReferenceExistsInMap(string keyName)
        {
            return References.Any(x => x.KeyName.ToLower().Trim() == keyName.ToLower().Trim());
        }

        bool ReferencesExistsInDB(string keyName, List<References> dbReferences)
        {
            return dbReferences.Any(x => x.KeyName.ToLower().Trim() == keyName.ToLower().Trim());
        }
        public string UpdateReferences(List<References> dbReferences)
        {
            string cmd = "";

            foreach (References dbref in dbReferences)
            {
                if (!ReferenceExistsInMap(dbref.KeyName))
                {
                    cmd += "ALTER TABLE " + Name + " DROP FOREIGN KEY `" + dbref.KeyName + "`, DROP INDEX `" + dbref.KeyName + "`;" + Environment.NewLine;
                }
            }

            foreach (References reference in References)
            {
                if (!ReferencesExistsInDB(reference.KeyName, dbReferences))
                {
                    cmd += "ALTER TABLE `" + Name + "` ADD CONSTRAINT `" + reference.KeyName + "` FOREIGN KEY (`" + reference.FieldName + "`) " +
                    "REFERENCES `" + reference.ForeignTableName + "` (`" + reference.ForeignFieldName + "`);";

                    if (reference.OnDeleteAction != OnDelete.Restrict)
                    {
                        cmd += " ON DELETE " + Utils.GetEnumDescription(reference.OnDeleteAction);
                    }

                    if (reference.OnUpdateAction != OnUpdate.Restrict)
                    {
                        cmd += " ON UPDATE " + Utils.GetEnumDescription(reference.OnUpdateAction);
                    }
                }
                else
                {
                    if (!reference.Equals(dbReferences.SingleOrDefault(x => x.FieldName == reference.KeyName)))
                    {
                        cmd += "ALTER TABLE " + Name + " DROP FOREIGN KEY `" + reference.KeyName + "`, DROP INDEX `" + reference.KeyName + "`;" + Environment.NewLine;
                        cmd += "ALTER TABLE `" + Name + "` ADD CONSTRAINT `" + reference.KeyName + "` FOREIGN KEY (`" + reference.FieldName + "`) " +
                    "REFERENCES `" + reference.ForeignTableName + "` (`" + reference.ForeignFieldName + "`);";

                        if (reference.OnDeleteAction != OnDelete.Restrict)
                        {
                            cmd += " ON DELETE " + Utils.GetEnumDescription(reference.OnDeleteAction);
                        }

                        if (reference.OnUpdateAction != OnUpdate.Restrict)
                        {
                            cmd += " ON UPDATE " + Utils.GetEnumDescription(reference.OnUpdateAction);
                        }
                    }
                }
            }

            return cmd;
        }


        public string GetReferencesFromServer()
        {
            string cmd = "SELECT TABLE_NAME, COLUMN_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_NAME, " +
            "REFERENCED_COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE " +
            "Table_name = '" + this.Name + "' AND table_schema = '" + this.DatabaseName + "' AND CONSTRAINT_NAME <> 'PRIMARY';";

            return cmd;
        }

        public string UpdateTable(Table mappedTable, Table databaseTable)
        {
            string cmd = "";

            foreach (Field field in mappedTable.Fields)
            {
                if (databaseTable.Fields.Any(x => x.Name == field.Name))
                {
                    Field dbField = databaseTable.Fields.Find(x => x.Name == field.Name);
                    FieldAction fieldAction = field.Compare(dbField);

                    if (fieldAction.Action == ActionType.Update)
                    {
                        cmd += "ALTER TABLE `" + mappedTable.Name + "` CHANGE COLUMN `" + field.Name + "` `" + field.Name + "` " + field.Type + " ";

                        if (!field.AllowNull)
                        {
                            if (!string.IsNullOrEmpty(field.Extra) && field.Extra.ToUpper() == "AUTO_INCREMENT")
                            {
                                cmd += "NOT NULL " + field.Extra + " ";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(field.Default))
                                {
                                    throw ExceptionManager.Exception("The default value must contain a value when not null.", null, field.Name);
                                }

                                cmd += "NOT NULL DEFAULT " + field.Default + " ";
                            }
                        }
                        else
                        {
                            cmd += string.IsNullOrEmpty(field.Default) ? "NULL " : "NULL DEFAULT " + field.Default + " ";
                        }

                        cmd += !string.IsNullOrEmpty(field.After) ? " AFTER " + field.After : "";

                        cmd += ";";
                    }
                }
                else
                {
                    cmd += "ALTER TABLE `" + mappedTable.Name + "` ADD COLUMN `" + field.Name + "` " + field.Type.ToUpper() + " ";

                    if (!field.AllowNull)
                    {
                        if (!string.IsNullOrEmpty(field.Extra) && field.Extra.ToUpper() == "AUTO_INCREMENT")
                        {
                            cmd += "NOT NULL " + field.Extra + " ";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(field.Default))
                            {
                                throw ExceptionManager.Exception("The default value must contain a value when not null.", null, field.Name);
                            }

                            cmd += "NOT NULL DEFAULT " + field.Default + " ";
                        }
                    }
                    else
                    {
                        cmd += string.IsNullOrEmpty(field.Default) ? "NULL " : "NULL DEFAULT " + field.Default + " ";
                    }

                    cmd += !string.IsNullOrEmpty(field.After) ? " AFTER " + field.After : "";
                    cmd += ";";
                }
            }

            return cmd;
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
                            if (field.Type.Contains("varchar"))
                            {
                                field.Default = "''";
                            }
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
                    "REFERENCES `" + reference.ForeignTableName + "` (`" + reference.ForeignFieldName + "`)";

                if (reference.OnDeleteAction != OnDelete.Restrict)
                {
                    cmd += " ON DELETE " + Utils.GetEnumDescription(reference.OnDeleteAction);
                }

                if (reference.OnUpdateAction != OnUpdate.Restrict)
                {
                    cmd += " ON UPDATE " + Utils.GetEnumDescription(reference.OnUpdateAction);
                }

                cmd += ",";
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

        public void AddIdField(string name)
        {
            FieldExists(name);

            Fields.Add(new Field()
            {
                Name = name,
                AllowNull = false,
                Type = "INT(11)",
                Key = "PRI",
                Extra = "AUTO_INCREMENT"
            });
        }

        public void AddIdField()
        {
            AddIdField("id");
        }

        public void AddIntField(string name, int size, bool allowNull = true, int defaultValue = -1, string after = "")
        {
            FieldExists(name);

            Fields.Add(new Field()
            {
                Name = name,
                AllowNull = allowNull,
                Type = "INT(" + size.ToString() + ")",
                Default = (defaultValue > -1 ? defaultValue.ToString() : ""),
                After = after
            });
        }

        public void AddIntField(string name)
        {
            AddIntField(name, 11, true, -1, "");
        }

        public void AddDateTimeField(string name)
        {
            FieldExists(name);

            Fields.Add(new Field()
            {
                Name = name,
                AllowNull = true,
                Default = "",
                Type = "DATETIME"
            });
        }
        public void AddVarCharField(string name, int size, string defaultValue = "", string after = "")
        {
            FieldExists(name);

            Fields.Add(new Field()
            {
                Name = name,
                AllowNull = true,
                Default = defaultValue,
                Type = "VARCHAR(" + size.ToString() + ")",
                After = after
            });
        }

        bool FieldExists(string name)
        {
            if (Fields.Any(x => x.Name == name))
            {
                throw new Exception("the field name " + name + " already exists");
            }

            return false;
        }
    }
}
