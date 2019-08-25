using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MyConnector.Mapping
{
    public class Database
    {
        public MyCon MyCon { get; set; }
        public string Name { get; set; }
        public List<Table> Tables { get; set; }

        public Database(string name, MyCon myCon) : this(name)
        {
            MyCon = myCon ?? throw new ArgumentNullException(nameof(myCon));
        }

        public Database()
        {
            GrantLists();
        }
        public Database(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GrantLists();
        }

        void GrantLists()
        {
            if (Tables is null)
            {
                Tables = new List<Table>();
            }
        }

        public void ValidateTable(string tableName)
        {
            Table table = new Table(tableName);
            Table mappedTable = Tables.Find(x => x.Name == tableName);

            if(mappedTable == null)
            {
                throw new Exception(tableName + " is not mapped");
            }

            if (MyCon.HasRows("show tables like '" + tableName + "';"))
            {
                string cmd = "describe " + tableName + ";";
                DataTable dt = MyCon.Select(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    table.Fields.Add(new Field()
                    {
                        Name = dt.Rows[i]["Field"].ToString(),
                        Type = dt.Rows[i]["Type"].ToString(),
                        AllowNull = dt.Rows[i]["Null"].ToString() == "YES" ? true : false,
                        Key = dt.Rows[i]["Key"].ToString(),
                        Default = dt.Rows[i]["Default"].ToString(),
                        Extra = dt.Rows[i]["Extra"].ToString()
                    });
                }

                Console.WriteLine("Comparing tables");
                string updateTable = mappedTable.UpdateTable(mappedTable, table);

                if (!string.IsNullOrEmpty(updateTable))
                {
                    MyCon.ExecuteTransaction(updateTable);
                }
            }
            else
            {
                MyCon.ExecuteTransaction(mappedTable.GetCreateTable());
            }
        }
    }
}
