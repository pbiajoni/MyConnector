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
        public Table()
        {
            if (Fields is null)
            {
                Fields = new List<Field>();
            }
        }

        public Table(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (Fields is null)
            {
                Fields = new List<Field>();
            }
        }

        public string GetCreateTable(Table table)
        {
            string cmd = "CREATE TABLE `" + table.Name + "` (";

            foreach (Field field in table.Fields)
            {
                cmd += "`" + field.Name + "` " + field.Type + " " + (!field.AllowNull ? "NOT NULL" : "") + " " + field.Extra + ",";
            }

            foreach (Field field in table.Fields)
            {
                if (field.Key.ToUpper() == "PRI")
                {
                    cmd += "PRIMARY KEY (`" + field.Name + "`),";
                }
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
