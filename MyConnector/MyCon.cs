﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConnector
{
    public class MyCon
    {
        public delegate void OnExecuteQueryEventHandler(string query);
        public event OnExecuteQueryEventHandler OnExecuteQuery;

        public delegate void OnOpenConnectionEventHandler();
        public event OnOpenConnectionEventHandler OnOpenConnection;

        public delegate void OnCloseConnectionEventHandler();
        public event OnCloseConnectionEventHandler OnCloseConnection;

        public delegate void OnCommitedEventHandler();
        public event OnCommitedEventHandler OnCommited;

        public delegate void OnRollBackEventHandler();
        public event OnRollBackEventHandler OnRollBack;

        MySqlConnection MySQLConn = new MySqlConnection();
        MySqlTransaction MySQLTran;
        MySqlCommand mySqlCommand;

        private string _connectionString;
        private string _dataBase;
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public bool ConsoleQueryEvents { get; set; }

        public bool IncludeSecurityAsserts { get; set; }
        public List<CustomError> CustomErrors { get; set; }
        public MyCon(string connectionString)
        {
            this._connectionString = connectionString;
            this.Database = GetDBName(connectionString);

            if (CustomErrors == null)
            {
                CustomErrors = new List<CustomError>();
            }
        }
        private string GetDBName(string connectionString)
        {
            string[] splited = connectionString.Split(';');

            foreach (string str in splited)
            {
                string[] param = str.Split('=');
                if (param[0].ToLower().Contains("database"))
                {
                    return param[1];
                }
            }

            return null;
        }
        public MyCon(string username, string password, string server, string database)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Port = 3306;
            Server = server ?? throw new ArgumentNullException(nameof(server));
            Database = database ?? throw new ArgumentNullException(nameof(database));

            if (CustomErrors == null)
            {
                CustomErrors = new List<CustomError>();
            }
        }
        public MyCon(string username, string password, int port, string server, string database)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Port = port;
            Server = server ?? throw new ArgumentNullException(nameof(server));
            Database = database ?? throw new ArgumentNullException(nameof(database));

            if (CustomErrors == null)
            {
                CustomErrors = new List<CustomError>();
            }
        }
        public string getQuery(string cmdSQL, string chavePrimaria)
        {

            string cmd = null;
            string[] q = cmdSQL.Trim().Split(' ');

            if (q[0].ToLower() == "insert")
            {
                cmd = "select " + chavePrimaria + " from " + q[2] + " order by " + chavePrimaria + " DESC limit 1;";
            }

            if (q[0].ToLower() == "update")
            {
                cmd = "select " + chavePrimaria + " from " + q[1] + " order by " + chavePrimaria + " DESC limit 1;";
            }

            return cmd;
        }
        public void Initialize()
        {
            if (String.IsNullOrEmpty(this._connectionString))
            {
                string include = "";

                if (this.IncludeSecurityAsserts)
                {
                    include = ",includesecurityassets=True";
                }

                this._connectionString = String.Format(@"allow zero datetime=false;allow user variables=true;Connect Timeout=30;SERVER={0};PORT={1};UID={2};PASSWORD={3};DATABASE={4}{5};", Server, Port, Username, Password, Database, include);
            }
            else
            {
                throw new Exception("The connection string is already initialized");
            }
        }
        void AvoidInjection(string cmd)
        {
            cmd = cmd.ToLower();
            if (cmd.Contains("drop table") || cmd.Contains("drop database") || cmd.Contains("truncate"))
            {
                throw new Exception("This command is not acceptable. Very bad command.");
            }
        }

        void ConsoleCmd(string cmd)
        {
            if (ConsoleQueryEvents)
            {
                Console.WriteLine("MyConnector: " + cmd);
            }
        }

        void ConsoleEvents(string str)
        {
            Console.WriteLine("MyConnector: " + str);
        }

        public async Task<DataTable> SelectWithParametersAsync(string cmd, List<MyParameter> parameters)
        {
            List<MySqlParameter> _params = new List<MySqlParameter>();

            foreach (MyParameter p in parameters)
            {
                _params.Add(new MySqlParameter(p.ParameterName, p.ParameterValue));
            }

            return await SelectWithParametersAsync(cmd, _params);
        }

        public async Task<DataTable> SelectWithParametersAsync(string cmd, List<MySqlParameter> parameters)
        {
            ConsoleCmd(cmd);

            AvoidInjection(cmd);
            MySqlConnectionStringBuilder myCommString = new MySqlConnectionStringBuilder(_connectionString);
            MySqlConnection Conn = new MySqlConnection(myCommString.ConnectionString);

            try
            {
                await Conn.OpenAsync();
                try { await Conn.BeginTransactionAsync(IsolationLevel.ReadUncommitted); }
                catch (Exception er) { }
                MySqlCommand c = new MySqlCommand(cmd, Conn);

                foreach (MySqlParameter parameter in parameters)
                {
                    c.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                }

                c.CommandTimeout = 600;
                MySqlDataAdapter da = new MySqlDataAdapter(c);
                DataTable dt = new DataTable();
                await da.FillAsync(dt);
                await Conn.CloseAsync();

                return dt;
            }
            catch (MySqlException er)
            {
                this.RollBack();

                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                }

                throw er;
            }
        }
        public async Task<DataTable> SelectAsync(string cmd)
        {
            ConsoleCmd(cmd);

            AvoidInjection(cmd);

            MySqlConnectionStringBuilder myCommString = new MySqlConnectionStringBuilder(_connectionString);
            MySqlConnection Conn = new MySqlConnection(myCommString.ConnectionString);

            try
            {
                await Conn.OpenAsync();
                try { await Conn.BeginTransactionAsync(IsolationLevel.ReadUncommitted); }
                catch (Exception er) { }
                MySqlCommand c = new MySqlCommand(cmd, Conn);
                c.CommandTimeout = 600;
                MySqlDataAdapter da = new MySqlDataAdapter(c);
                DataTable dt = new DataTable();
                await da.FillAsync(dt);
                await Conn.CloseAsync();

                return dt;
            }
            catch (MySqlException er)
            {
                this.RollBack();

                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                }

                throw er;
            }
        }
        public DataTable Select(string cmdSQL)
        {
            ConsoleCmd(cmdSQL);

            if (this.OnExecuteQuery != null)
            {
                this.OnExecuteQuery(cmdSQL);
            }

            MySqlConnectionStringBuilder myCommString = new MySqlConnectionStringBuilder(_connectionString);
            MySqlConnection Conn = new MySqlConnection(myCommString.ConnectionString);

            try
            {
                Conn.Open();
                try { Conn.BeginTransaction(IsolationLevel.ReadUncommitted); }
                catch (Exception er) { }
                MySqlCommand c = new MySqlCommand(cmdSQL, Conn);
                c.CommandTimeout = 600;
                MySqlDataAdapter da = new MySqlDataAdapter(c);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Conn.Close();

                return dt;
            }
            catch (MySqlException er)
            {
                this.RollBack();

                if (Conn.State != ConnectionState.Closed)
                {
                    Conn.Close();
                }

                throw er;
            }
        }
        public void Commit()
        {
            if (MySQLConn.State != ConnectionState.Closed)
            {
                MySQLTran.Commit();
                ConsoleEvents("Commited");

                if (OnCommited != null)
                {
                    OnCommited();
                }
            }

            if (MySQLConn.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }
        public void CloseConnection()
        {
            if (MySQLConn.State != ConnectionState.Closed)
            {
                MySQLConn.Close();
                ConsoleEvents("Connection Closed");

                if (OnCloseConnection != null)
                {
                    OnCloseConnection();
                }
            }
        }
        public void RollBack()
        {

            if (MySQLConn.State != ConnectionState.Closed)
            {
                MySQLTran.Rollback();
                ConsoleEvents("RollBack");

                if (OnRollBack != null)
                {
                    OnRollBack();
                }
            }

            if (MySQLConn.State != ConnectionState.Closed)
            {
                this.CloseConnection();
            }
        }
        public async Task<bool> OpenConnectionAsync()
        {
            try
            {

                if (MySQLConn.State == ConnectionState.Closed)
                {
                    if (string.IsNullOrEmpty(_connectionString))
                    {
                        throw new Exception("BiaORM MySQL is not Initialized");
                    }

                    MySQLConn = new MySqlConnection(_connectionString);
                    await MySQLConn.OpenAsync();

                    MySQLTran = await MySQLConn.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

                    if (MySQLConn.State == ConnectionState.Open)
                    {
                        ConsoleEvents("Connection is now openned");
                        if (this.OnOpenConnection != null)
                        {
                            this.OnOpenConnection();
                        }

                        return true;
                    }
                    else if (MySQLConn.State == ConnectionState.Closed)
                    {
                        return false;
                    }

                    return false;
                }

                return true;
            }
            catch (MySqlException es)
            {
                throw es;
            }

        }
        public bool OpenConnection()
        {
            try
            {

                if (MySQLConn.State == ConnectionState.Closed)
                {
                    if (string.IsNullOrEmpty(_connectionString))
                    {
                        throw new Exception("BiaORM MySQL is not Initialized");
                    }

                    MySQLConn = new MySqlConnection(_connectionString);
                    MySQLConn.Open();

                    MySQLTran = MySQLConn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    if (MySQLConn.State == ConnectionState.Open)
                    {
                        ConsoleEvents("Connection is now openned");
                        if (this.OnOpenConnection != null)
                        {
                            this.OnOpenConnection();
                        }

                        return true;
                    }
                    else if (MySQLConn.State == ConnectionState.Closed)
                    {
                        return false;
                    }

                    return false;
                }

                return true;
            }
            catch (MySqlException es)
            {
                throw es;
            }

        }
        string GetErrorMessage(int errorCode)
        {
            //errors.Add(new Error() { Code = 1451, Message = "Este registro não pode ser removido pois contém dependências" });
            if (CustomErrors.Where(x => x.Code == errorCode).FirstOrDefault() == null)
            {
                return null;
            }

            return CustomErrors.Find(x => x.Code == errorCode).Message;
        }
        public async void ExecuteTransactionAsync(QueryBuilder queryBuilder)
        {
            this.ExecuteWithParametersAsync(queryBuilder.GetCommand(), queryBuilder.GetParameters());
        }
        public async void ExecuteWithParametersAsync(string cmd, List<MyParameter> parameters)
        {
            List<MySqlParameter> _params = new List<MySqlParameter>();

            foreach (MyParameter p in parameters)
            {
                _params.Add(new MySqlParameter(p.ParameterName, p.ParameterValue));
            }

            ExecuteWithParametersAsync(cmd, _params);
        }
        public async void ExecuteWithParametersAsync(string cmd, List<MySqlParameter> parameters)
        {
            try
            {
                await OpenConnectionAsync();

                if (mySqlCommand is null)
                {
                    MySqlCommand c = new MySqlCommand();
                    c.Connection = MySQLConn;
                    c.CommandText = cmd;

                    foreach (MySqlParameter parameter in parameters)
                    {
                        c.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    }

                    c.CommandTimeout = 3600;
                    c.Transaction = MySQLTran;
                    ConsoleCmd(cmd);
                    await c.ExecuteNonQueryAsync();
                }
                else
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        mySqlCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    }

                    mySqlCommand.Connection = MySQLConn;
                    mySqlCommand.CommandText = cmd;
                    mySqlCommand.CommandTimeout = 3600;
                    mySqlCommand.Transaction = MySQLTran;
                    ConsoleCmd(cmd);
                    await mySqlCommand.ExecuteNonQueryAsync();
                    mySqlCommand = null;
                }
            }
            catch (MySqlException mException)
            {
                this.RollBack();
                string errorMessage = GetErrorMessage(mException.Number);
                Exception exception = null;
                string command = "Fail Command -> " + cmd;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    exception = new Exception(errorMessage, mException);
                }
                else
                {
                    exception = new Exception(mException.Message, mException);
                }

                ConsoleCmd(command);
                ConsoleEvents(exception.ToString());
                throw exception;
            }
        }
        public async void ExecuteTransactionAsync(string cmdSQL)
        {
            try
            {
                await OpenConnectionAsync();
                ConsoleCmd(cmdSQL);

                if (this.OnExecuteQuery != null)
                {
                    this.OnExecuteQuery(cmdSQL);
                }

                if (mySqlCommand is null)
                {
                    MySqlCommand c = new MySqlCommand(cmdSQL, MySQLConn);
                    c.CommandTimeout = 3600;
                    c.Transaction = MySQLTran;
                    await c.ExecuteNonQueryAsync();
                }
                else
                {
                    mySqlCommand.CommandText = cmdSQL;
                    mySqlCommand.Connection = MySQLConn;
                    mySqlCommand.CommandTimeout = 3600;
                    mySqlCommand.Transaction = MySQLTran;
                    await mySqlCommand.ExecuteNonQueryAsync();
                    mySqlCommand = null;
                }
            }
            catch (MySqlException mException)
            {
                this.RollBack();
                string errorMessage = GetErrorMessage(mException.Number);
                Exception exception = null;
                string command = "Fail Command -> " + cmdSQL;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    exception = new Exception(errorMessage, mException);
                }
                else
                {
                    exception = new Exception(mException.Message, mException);
                }

                ConsoleCmd(command);
                ConsoleEvents(exception.ToString());
                throw exception;
            }
        }
        public void ExecuteTransaction(string cmdSQL)
        {
            try
            {
                OpenConnection();
                ConsoleCmd(cmdSQL);

                if (this.OnExecuteQuery != null)
                {
                    this.OnExecuteQuery(cmdSQL);
                }

                if (mySqlCommand is null)
                {
                    MySqlCommand c = new MySqlCommand(cmdSQL, MySQLConn);
                    c.CommandTimeout = 3600;
                    c.Transaction = MySQLTran;
                    c.ExecuteNonQuery();
                }
                else
                {
                    mySqlCommand.CommandText = cmdSQL;
                    mySqlCommand.Connection = MySQLConn;
                    mySqlCommand.CommandTimeout = 3600;
                    mySqlCommand.Transaction = MySQLTran;
                    mySqlCommand.ExecuteNonQuery();
                    mySqlCommand = null;
                }
            }
            catch (MySqlException mException)
            {
                this.RollBack();
                string errorMessage = GetErrorMessage(mException.Number);
                Exception exception = null;
                string command = "Fail Command -> " + cmdSQL;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    exception = new Exception(errorMessage, mException);
                }
                else
                {
                    exception = new Exception(mException.Message, mException);
                }

                ConsoleCmd(command);
                ConsoleEvents(exception.ToString());
                throw exception;
            }
        }
        public void NewCommand()
        {
            mySqlCommand = new MySqlCommand();
        }
        public void AddParameter(string parameterName, object value)
        {
            mySqlCommand.Parameters.AddWithValue(parameterName, value);
        }
        /// <summary>
        /// Retorna a Id do registro criado
        /// </summary>
        /// <param name="cmdSQL"></param>
        /// <param name="fieldReturn"></param>
        /// <returns></returns>
        public string ExecuteTransaction(string cmdSQL, string fieldReturn)
        {
            ExecuteTransaction(cmdSQL);
            DataTable dt = Select(getQuery(cmdSQL, fieldReturn));

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][fieldReturn].ToString();
            }

            return null;
        }
        public async Task<string> ExecuteTransactionAsync(string cmdSQL, string fieldReturn)
        {
            ExecuteTransactionAsync(cmdSQL);
            DataTable dt = await SelectAsync(getQuery(cmdSQL, fieldReturn));

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][fieldReturn].ToString();
            }

            return null;
        }
        public bool Exists(string tableName, string fieldName, string value, string Id = null)
        {
            string stmt = "";

            if (Id != null)
            {
                stmt = " and Id <> '" + Id + "' ";
            }

            string cmd = "select Id from " + tableName + " where " + fieldName + " = '" + value + "' " + stmt + ";";
            return HasRows(cmd);
        }
        public bool HasRows(string cmdSQL)
        {
            DataTable dt = Select(cmdSQL);

            if (dt.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
