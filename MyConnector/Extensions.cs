using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MyConnector
{
    public static class Extensions
    {
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
