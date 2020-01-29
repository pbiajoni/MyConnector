using MyConnector.Mapping;
using MySql.Data.MySqlClient;
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
        public object Id { get; set; }
        private string _idFieldName { get; set; }

        public string IdFieldName
        {
            get
            {
                return _idFieldName;
            }

            set
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    _idFieldName = "id";
                }
                else
                {
                    _idFieldName = value;
                }
            }
        }

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


        public List<MySqlParameter> GetParameters()
        {

            if(this.Items.Count == 0)
            {
                throw new Exception("Query Build Items can not be null");
            }

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            foreach(QueryBuilderItem item in this.Items)
            {
                parameters.Add(new MySqlParameter(item.FieldName, item.Value));
            }

            return parameters;
        }

        public string GetCommand()
        {
            return GetCommand(this.QueryType);
        }

        public string GetCommand(QueryType queryType)
        {
            if (queryType == QueryType.Insert)
            {
                return Insert();
            }

            if (queryType == QueryType.Update)
            {
                return Update(this.Id, this.IdFieldName);
            }

            throw new Exception("Query Type must be different than None");
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
