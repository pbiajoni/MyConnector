using MyConnector.Mapping;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MyConnector
{
    public class QueryBuilder
    {
        public Table TableMap { get; set; }
        public List<QueryBuilderItem> Items { get; set; }
        public QueryType QueryType { get; set; }
        public object Id { get; set; }
        private string _idFieldName { get; set; }
        private MyCon _myCon;

        public MyCon Connector
        {
            get
            {
                return this._myCon;
            }

            set
            {
                this._myCon = value;
            }
        }

        public string IdFieldName
        {
            get
            {
                if (string.IsNullOrEmpty(_idFieldName))
                {
                    _idFieldName = "id";
                }

                return _idFieldName;
            }

            set
            {
                _idFieldName = value;
            }
        }

        public QueryBuilder()
        {
            GrantList();
        }

        public QueryBuilder(Table table, QueryType queryType = QueryType.None)
        {
            TableMap = table;
            QueryType = QueryType;
            GrantList();
        }

        public QueryBuilder(Table table, MyCon myCon)
        {
            TableMap = table;
            QueryType = QueryType.None;
            _myCon = myCon;
            GrantList();
        }

        public QueryBuilder(Table table, MyCon myCon, QueryType queryType = QueryType.None)
        {
            TableMap = table;
            QueryType = QueryType;
            _myCon = myCon;
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

            if (this.Items.Count == 0)
            {
                throw new Exception("Query Build Items can not be null");
            }

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            foreach (QueryBuilderItem item in this.Items)
            {
                parameters.Add(new MySqlParameter("@" + item.FieldName, item.Value));
            }

            if (this.QueryType == QueryType.Update)
            {
                if (!parameters.Exists(x => x.ParameterName == this.IdFieldName))
                {
                    parameters.Add(new MySqlParameter("@" + this.IdFieldName, this.Id));
                }
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

        public string GetCommandWithParameters()
        {
            return GetCommandWithParameters(this.QueryType);
        }

        public string GetCommandWithParameters(QueryType queryType)
        {
            if (queryType == QueryType.Insert)
            {
                return InsertWithParameters();
            }

            if (queryType == QueryType.Update)
            {
                return UpdateWithParameters();
            }

            throw new Exception("Query Type must be different than None");
        }


        public async Task<string> ExecuteWithParametersAsync(string fieldToReturn = "id")
        {
            if (this.QueryType == QueryType.Insert)
            {
                return await ExecuteInsertWithParametersAsync(fieldToReturn);
            }

            if (this.QueryType == QueryType.Update)
            {
                ExecuteUpdateWithParameters();
            }

            if (this.QueryType == QueryType.Delete)
            {
                ExecuteDeleteWithParametersAsync();
            }

            throw new Exception("Query Type can not be none");
        }

        public async Task<string> ExecuteInsertWithParametersAsync(string fieldToReturn = "id")
        {
            this._myCon.ExecuteWithParametersAsync(this.InsertWithParameters(), this.GetParameters());
            DataTable dt = await _myCon.SelectAsync(_myCon.getQuery(this.InsertWithParameters(), fieldToReturn));

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][fieldToReturn].ToString();
            }

            return null;
        }
        public void ExecuteInsert()
        {
            this._myCon.ExecuteWithParametersAsync(this.InsertWithParameters(), this.GetParameters());
        }

        public void ExecuteUpdateWithParameters()
        {
            this._myCon.ExecuteWithParametersAsync(this.UpdateWithParameters(this.IdFieldName), this.GetParameters());
        }

        public void ExecuteDeleteWithParametersAsync()
        {
            this._myCon.ExecuteWithParametersAsync(this.DeleteWithParameters(), GetParameters());
        }

        public async Task<DataTable> SelectWithParametersAsync(string cmd)
        {
            if (this.TableMap != null)
            {
                cmd = cmd.Replace("@tablename", this.TableMap.Name);
            }

            return await this._myCon.SelectWithParametersAsync(cmd, GetParameters());
        }

        public string InsertWithParameters()
        {
            string cmd = "INSERT INTO `" + TableMap.Name + "` ";
            string columns = "";
            string values = "";

            foreach (QueryBuilderItem item in Items)
            {
                columns += item.FieldName + ",";

                if (!item.IsMD5)
                {
                    values += "@" + item.FieldName + ",";
                }
                else
                {
                    values += "MD5(@" + item.FieldName + "),";
                }
            }

            return cmd + ("(" + columns.TrimEnd(',') + ") values (" + values.TrimEnd(',') + ");");
        }

        public string UpdateWithParameters()
        {
            if (this.Id is null)
            {
                throw new Exception("ID can not be null");
            }

            if (string.IsNullOrEmpty(this.IdFieldName))
            {
                throw new Exception("ID Field Name can not be null");
            }

            return UpdateWithParameters(this._idFieldName.ToString());
        }


        public string DeleteWithParameters()
        {
            string cmd = "DELETE FROM `" + TableMap.Name + "` WHERE ";
            string concat = " AND ";

            if (this.Items.Count > 0)
            {
                cmd += " WHERE ";
                foreach (QueryBuilderItem item in this.Items)
                {
                    cmd += item.FieldName.Trim() + " = " + "@" + item.FieldName.Trim() + " " + concat;
                }
            }
            else
            {
                concat = " WHERE ";
            }

            cmd += _idFieldName + " = " + this.Id;
            return cmd;
        }

        public string UpdateWithParameters(string identifier = "id")
        {
            string cmd = "UPDATE `" + TableMap.Name + "` SET ";

            foreach (QueryBuilderItem item in Items)
            {
                if (item.FieldName != identifier)
                {
                    if (!item.IsMD5)
                    {
                        cmd += item.FieldName.Trim() + " = " + "@" + item.FieldName.Trim() + ",";
                    }
                    else
                    {
                        cmd += item.FieldName.Trim() + " = " + "MD5(@" + item.FieldName.Trim() + "),";
                    }
                }
            }

            cmd = cmd.TrimEnd(',');
            cmd += " WHERE " + identifier.Trim() + " = @" + identifier.ToString() + ";";

            return cmd;
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
