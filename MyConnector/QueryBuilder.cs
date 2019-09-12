using MyConnector.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector
{
    public class QueryBuilder
    {
        public Table TableMap { get; set; }
        public List<QueryBuilderItem> Items { get; set; }
        public QueryBuilder()
        {
            GrantList();
        }

        public QueryBuilder(Table table)
        {
            TableMap = table;
            GrantList();
        }

        public QueryBuilder(string tableName)
        {
            TableMap = new Table(tableName);
            GrantList();
        }

        private void GrantList()
        {
            if (Items is null)
            {
                Items = new List<QueryBuilderItem>();
            }
        }

        public string Update(object id)
        {
            throw new NotImplementedException();
        }
        public string Insert()
        {
            string cmd = "INSERT INTO `" + TableMap.Name + "` ";
            string columns = "";
            string values = "";

            foreach (QueryBuilderItem item in Items)
            {
                columns += item.FieldName + ",";

                if (item.AddSlash)
                {
                    values += "'" + item.Value + "',";
                }
                else
                {
                    values += "'" + Utils.AddSlash(item.Value.ToString()) + "',";
                }
            }

            return cmd + ("(" + columns.TrimEnd(',') + ") values (" + values.TrimEnd(',') + ");");
        }
    }
}
