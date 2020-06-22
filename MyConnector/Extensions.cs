using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MyConnector
{
    public static class Extensions
    {
        public static int ToInt32(this string value)
        {
            return Convert.ToInt32(value);
        }

        public static List<int> ToIdsIn(this DataTable dt, string fieldName = "id")
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int id = dt.ToRowInt(fieldName, i);
                if (!ids.Contains(id))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        public static List<string> ToStringListOfIds(this DataTable dataTable, string idFieldName = "id")
        {
            List<string> ids = new List<string>();

            if (dataTable.Columns.Contains(idFieldName))
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ids.Add(dataTable.ToRowString(idFieldName, i));
                }

                return ids;
            }

            throw new Exception(idFieldName + " does not exists in DataTable Columns");
        }

        public static List<int> ToIntListOfIds(this DataTable dataTable, string idFieldName = "id")
        {
            List<string> ids = ToStringListOfIds(dataTable, idFieldName);
            List<int> int_ids = new List<int>();

            foreach (string id in ids)
            {
                int_ids.Add(Convert.ToInt32(id));
            }

            return int_ids;
        }

        public static T ToOneOf<T>(this DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                return default(T);
            }

            T obj = Activator.CreateInstance<T>();

            foreach (var p in obj.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
            {
                if (p != null && p.CanWrite)
                {
                    string name = p.Name;
                    if (dataTable.Columns.Contains(p.Name))
                    {
                        if ((dataTable.Rows[0][p.Name] != DBNull.Value))
                        {
                            p.SetValue(obj, dataTable.Rows[0][p.Name]);
                        }
                    }
                }
            }

            return obj;
        }

        public static List<T> TolistOf<T>(this DataTable dataTable)
        {
            List<T> list = Activator.CreateInstance<List<T>>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                T obj = Activator.CreateInstance<T>();

                foreach (var p in obj.GetType().GetProperties().Where(p => !p.GetGetMethod().GetParameters().Any()))
                {
                    if (p != null && p.CanWrite)
                    {
                        string name = p.Name;
                        if (dataTable.Columns.Contains(p.Name))
                        {
                            if ((dataTable.Rows[i][p.Name] != DBNull.Value))
                            {
                                object value = dataTable.Rows[i][p.Name];
                                //Console.WriteLine("Writing value " + value.ToString() + " for " + name + " with type " + value.GetType().ToString());
                                p.SetValue(obj, value);
                            }
                        }
                    }
                }

                list.Add(obj);
            }

            return list;
        }


        public static void Add(this QueryBuilder qb, string fieldName, object value, bool addSlash = false, bool removeSingleQuotes = false)
        {
            QueryBuilderItem item = new QueryBuilderItem(fieldName, value, addSlash);
            item.RemoveSingleQuotes = removeSingleQuotes;
            qb.Items.Add(item);
        }

        public static void AddParameter(this QueryBuilder qb, string parameterName, object value, bool IsMD5 = false)
        {
            QueryBuilderItem item = new QueryBuilderItem(parameterName, value);
            item.IsMD5 = IsMD5;
            qb.Items.Add(item);
        }

        public static void AddParameterCase(this QueryBuilder qb, string parameterName, object value, bool addCase = true, bool IsMD5 = false)
        {
            if (addCase)
            {
                QueryBuilderItem item = new QueryBuilderItem(parameterName, value);
                item.IsMD5 = IsMD5;
                qb.Items.Add(item);
            }
        }

        public static void AddDateTimeParameter(this QueryBuilder qb, string parameterName, DateTime value)
        {
            QueryBuilderItem item = new QueryBuilderItem(parameterName, value.ToDBDateTime());
            qb.Items.Add(item);
        }

        /// <summary>
        /// Add if datetime has value
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public static void AddDateTimeParameterCase(this QueryBuilder qb, string parameterName, DateTime? value)
        {
            if (value.HasValue)
            {
                QueryBuilderItem item = new QueryBuilderItem(parameterName, value.Value.ToDBDateTime());
                qb.Items.Add(item);
            }
        }

        public static void AddIdParameter(this QueryBuilder qb, object value, string parameterName = "id")
        {
            QueryBuilderItem item = new QueryBuilderItem(parameterName, value);
            qb.Items.Add(item);
        }

        public static void AddNowParameter(this QueryBuilder qb, string parameterName)
        {
            QueryBuilderItem item = new QueryBuilderItem(parameterName, DateTime.Now.ToDBDateTime());
            qb.Items.Add(item);
        }

        public static string ToDBDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static bool HasRows(this DataTable dt)
        {
            return dt.Rows.Count > 0 ? true : false;
        }
        public static int ToRowId(this DataTable dt, int index = 0, string column = "id")
        {
            return Convert.ToInt32(dt.Rows[index][column].ToString());
        }

        public static string ToRowValue(this DataTable dt, string column, int index = 0)
        {
            return dt.Rows[index][column].ToString();
        }

        public static string ToRowString(this DataTable dt, string column, int index = 0)
        {
            return dt.Rows[index][column].ToString();
        }


        public static int ToRowInt(this DataTable dt, string column, int index = 0, int defaultValue = 0)
        {
            string value = dt.Rows[index][column].ToString();

            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue.ToString();
            }

            return Convert.ToInt32(value);
        }

        public static DateTime? ToRowNullableDateTime(this DataTable dt, string column, int index = 0)
        {
            string dtm = dt.Rows[index][column].ToString();

            if (!string.IsNullOrEmpty(dtm))
            {
                return Convert.ToDateTime(dt.Rows[index][column].ToString());
            }

            return null;
        }

        public static DateTime ToRowDateTime(this DataTable dt, string column, int index = 0)
        {
            return Convert.ToDateTime(dt.Rows[index][column].ToString());
        }

        public static DateTime ToDate(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }
    }
}
