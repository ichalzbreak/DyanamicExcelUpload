using System.Data;

namespace DynamicExcelUpload.Core.Helpers
{
    public class QueryHelper
    {
        public static string GetQueryData(string tableName)
        {
            return $"SELECT * FROM [dbo].[{tableName.Trim()}]";
        }

        public static string GetQueryCreateTable(string tableName, DataTable table)
        {
            string query = string.Empty;
            query += "IF EXISTS (SELECT * FROM sys.objects WHERE object_id = ";
            query += $"OBJECT_ID(N'[dbo].[{tableName.Trim()}]') AND type in (N'U'))";
            query += $"DROP TABLE [dbo].[{tableName.Trim()}]";

            query += " CREATE TABLE " + tableName.Trim() + "(";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                query += "\n [" + table.Columns[i].ColumnName.Trim() + "] ";
                string columnType = table.Columns[i].DataType.ToString();
                switch (columnType)
                {
                    case "System.Int32":
                        query += " int ";
                        break;
                    case "System.Int64":
                        query += " bigint ";
                        break;
                    case "System.Int16":
                        query += " smallint";
                        break;
                    case "System.Byte":
                        query += " tinyint";
                        break;
                    case "System.Decimal":
                        query += " decimal ";
                        break;
                    case "System.DateTime":
                        query += " datetime ";
                        break;
                    case "System.String":
                    default:
                        query += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "255" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    query += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    query += " NOT NULL ";
                query += ",";
            }
            return query.Substring(0, query.Length - 1) + "\n)";
        }
    }
}
