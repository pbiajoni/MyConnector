using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MyConnector.Mapping
{
    public class Database
    {
        public delegate void ValidateTableEventHandler(Table table);
        public event ValidateTableEventHandler OnValidateTable;
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

        public Table GetTable(string tableName)
        {
            return Tables.Single(x => x.Name == tableName);
        }
        public void ValidateAllTables()
        {
            if(Tables.Count == 0)
            {
                throw new Exception("There are no tables to validate");
            }


            foreach (Table table in Tables)
            {
                if (OnValidateTable != null) { OnValidateTable(table); }                
                ValidateTable(table.Name);
            }
        }

        public List<References> GetReferencesFromServer(Table table)
        {
            Console.WriteLine("getting references from server of table " + table.Name);
            string cmd = table.GetReferencesFromServer();
            DataTable dt = MyCon.Select(cmd);

            List<References> references = new List<References>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string column_name = dt.Rows[i]["COLUMN_NAME"].ToString();
                string constraint_name = dt.Rows[i]["CONSTRAINT_NAME"].ToString();
                string referenced_table_name = dt.Rows[i]["REFERENCED_TABLE_NAME"].ToString();
                string referenced_column_name = dt.Rows[i]["REFERENCED_COLUMN_NAME"].ToString();

                references.Add(new References(referenced_table_name,
                    referenced_column_name, column_name, constraint_name));
            }

            return references;
        }

        public void ValidateTable(string tableName)
        {
            Table table = new Table(tableName);
            table.DatabaseName = this.Name;
            Table mappedTable = Tables.Find(x => x.Name == tableName);

            if (mappedTable == null)
            {
                throw new Exception(tableName + " is not mapped");
            }

            if (MyCon.HasRows("show tables like '" + tableName + "';"))
            {
                string cmd = "describe `" + tableName + "`;";
                DataTable dt = MyCon.Select(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    table.Fields.Add(new Field()
                    {
                        Name = dt.Rows[i]["Field"].ToString(),
                        Type = dt.Rows[i]["Type"].ToString(),
                        AllowNull = dt.Rows[i]["Null"].ToString() == "YES" ? true : false,
                        Key = dt.Rows[i]["Key"].ToString(),
                        Default =  dt.Rows[i]["Default"].ToString(),
                        Extra = dt.Rows[i]["Extra"].ToString()
                    });
                }

                Console.WriteLine("Comparing tables");
                string updateTable = mappedTable.UpdateTable(mappedTable, table);
                string updateReferences = mappedTable.UpdateReferences(GetReferencesFromServer(table));

                if (!string.IsNullOrEmpty(updateTable))
                {
                    MyCon.ExecuteTransaction(updateTable);
                }

                if (!string.IsNullOrEmpty(updateReferences))
                {
                    MyCon.ExecuteTransaction(updateReferences);
                }
            }
            else
            {
                MyCon.ExecuteTransaction(mappedTable.GetCreateTable());

                foreach (string cmd in mappedTable.PrePopulateCommands)
                {
                    MyCon.ExecuteTransaction(cmd.Replace("@tablename", mappedTable.Name)
                        .Replace("@databasename", Name));
                }
            }
        }
    }
}
