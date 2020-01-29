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
        public QueryType QueryType { get; set; }

        public QueryBuilder()
        {
            GrantList();
        }

        public QueryBuilder(Table table)
        {
            TableMap = table;
            QueryType = QueryType.None;
            GrantList();
        }

        public QueryBuilder(Table table, QueryType queryType)
        {
            TableMap = table;
            QueryType = QueryType;
            GrantList();
        }

        public QueryBuilder(string tableName)
        {
            TableMap = new Table(tableName);
            QueryType = QueryType.None;
            GrantList();
        }

        private void GrantList()
        {
            if (Items is null)
            {
                Items = new List<QueryBuilderItem>();
            }
        }

        public string Update(object id, string identifier = "id")
        {
            string cmd = "UPDATE `" + TableMap.Name + "` SET ";


            foreach (QueryBuilderItem item in Items)
            {
                if (item.RemoveSingleQuotes)
                {
                    if (item.AddSlash)
                    {
                        cmd += item.FieldName.Trim() + " = " + Utils.AddSlash(item.Value.ToString().Trim()) + ",";
                    }
                    else
                    {
                        cmd += item.FieldName.Trim() + " = " + item.Value.ToString().Trim() + ",";
                    }
                }
                else
                {
                    if (item.AddSlash)
                    {
                        cmd += item.FieldName.Trim() + " = '" + Utils.AddSlash(item.Value.ToString().Trim()) + "',";
                    }
                    else
                    {
                        cmd += item.FieldName.Trim() + " = '" + item.Value.ToString().Trim() + "',";
                    }
                }

            }

            cmd = cmd.TrimEnd(',');
            cmd += " WHERE " + identifier.Trim() + " = " + id.ToString().Trim() + ";";
            return cmd;
        }
        public string Insert()
        {
            string cmd = "INSERT INTO `" + TableMap.Name + "` ";
            string columns = "";
            string values = "";

            foreach (QueryBuilderItem item in Items)
            {
                columns += item.FieldName + ",";

                if (item.RemoveSingleQuotes)
                {
                    if (!item.AddSlash)
                    {
                        values += item.Value.ToString().Trim() + ",";
                    }
                    else
                    {
                        values += Utils.AddSlash(item.Value.ToString()) + ",";
                    }
                }
                else
                {
                    if (!item.AddSlash)
                    {
                        values += "'" + item.Value.ToString().Trim() + "',";
                    }
                    else
                    {
                        values += "'" + Utils.AddSlash(item.Value.ToString()) + "',";
                    }
                }
            }

            return cmd + ("(" + columns.TrimEnd(',') + ") values (" + values.TrimEnd(',') + ");");
        }
    }
}
