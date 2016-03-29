using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InsertScriptFormatter
{
    class Program
    {
        private const string INSERT_PATTERN = @"^INSERT INTO (\w+) \((.*)\) VALUES \((.*)\);$";

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("InsertAllData.sql", Encoding.UTF8);

            Dictionary<string, List<InsertData>> insertsByTable = new Dictionary<string, List<InsertData>>();
            List<object> lineDataList = lines.Select(l => GetLineData(l, insertsByTable)).ToList();

            Dictionary<string, List<int>> columnStringValueWidthsByTable = new Dictionary<string, List<int>>(insertsByTable.Count);
            foreach (var insertsByTableEntry in insertsByTable)
            {
                string tableName = insertsByTableEntry.Key;
                List<int> columnStringValueWidths = GetColumnStringValueWidthsForTable(insertsByTableEntry.Value);
                columnStringValueWidthsByTable.Add(tableName, columnStringValueWidths);
            }

            string[] outputLines = lineDataList.Select(ld => GetOutputLine(ld, columnStringValueWidthsByTable)).ToArray();
            File.WriteAllLines("output.txt", outputLines, Encoding.UTF8);
        }

        private static object GetLineData(string line, Dictionary<string, List<InsertData>> insertsByTable)
        {
            Match match = Regex.Match(line, INSERT_PATTERN);
            if (match.Success)
            {
                GroupCollection groups = match.Groups;
                string tableName = groups[1].Value;
                string columns = groups[2].Value;
                string values = groups[3].Value;
                InsertData insertData = new InsertData(tableName, columns, values);

                List<InsertData> insertDataList;
                if (!insertsByTable.TryGetValue(tableName, out insertDataList))
                {
                    insertDataList = new List<InsertData>();
                    insertsByTable.Add(tableName, insertDataList);
                }

                insertDataList.Add(insertData);

                return insertData;
            }
            else
            {
                return line;
            }
        }

        private static List<int> GetColumnStringValueWidthsForTable(List<InsertData> insertDataList)
        {
            int numColumns = insertDataList[0].ColumnNames.Count;
            List<int> columnStringValueWidths = new List<int>(numColumns);
            for (int i = 0; i < numColumns; i++)
            {
                int maxWidth = insertDataList.Max(idt => idt.Values[i].Length);
                columnStringValueWidths.Add(maxWidth);
            }

            return columnStringValueWidths;
        }

        private static string GetOutputLine(object lineData, Dictionary<string, List<int>> columnStringValueWidthsByTable)
        {
            if (lineData is InsertData)
            {
                InsertData insertData = (InsertData)lineData;
                return insertData.GenerateString(columnStringValueWidthsByTable[insertData.TableName]);
            }
            else
            {
                return (string)lineData;
            }
        }

        private class InsertData
        {
            public string TableName { get; set; }
            public List<string> ColumnNames { get; set; }
            public List<string> Values { get; set; }

            public InsertData(string tableName, string columns, string values)
            {
                TableName = tableName;
                ColumnNames = columns
                    .Split(',')
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();

                List<int> valueSeparatorIndexes = new List<int>();
                valueSeparatorIndexes.Add(0);
                bool isInString = false;
                for (int i = 0; i < values.Length; )
                {
                    if (values[i] == '\'')
                    {
                        if (i < values.Length - 1 && values[i + 1] == '\'')
                        {
                            i += 2;
                        }
                        else
                        {
                            isInString = !isInString;
                            i++;
                        }
                    }
                    else if (values[i] == ',' && !isInString)
                    {
                        valueSeparatorIndexes.Add(i);
                        i++;
                    }
                    else
                    {
                        i++;
                    }
                }
                valueSeparatorIndexes.Add(values.Length);

                Values = new List<string>(ColumnNames.Count);
                for (int i = 0; i < valueSeparatorIndexes.Count - 1; i++)
                {
                    string value = values.Substring(valueSeparatorIndexes[i], valueSeparatorIndexes[i + 1] - valueSeparatorIndexes[i]);
                    value = value.Trim(' ', ',');
                    Values.Add(value);
                }

                if (!IsValid())
                {
                    throw new Exception();
                }
            }

            private bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(TableName) && ColumnNames != null && Values != null && ColumnNames.Count > 0 && Values.Count > 0 && ColumnNames.Count == Values.Count;
            }

            public string GenerateString(List<int> valueStringWidths)
            {
                return string.Format("INSERT INTO {0} ({1}) VALUES ({2});", TableName, GetColumnNamesString(), GetValuesString(valueStringWidths));
            }

            private string GetColumnNamesString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(ColumnNames[0]);
                for (int i = 1; i < ColumnNames.Count; i++)
                {
                    sb.Append(", " + ColumnNames[i]);
                }
                return sb.ToString();
            }

            private string GetValuesString(List<int> valueStringWidths)
            {
                if (valueStringWidths == null || valueStringWidths.Count != Values.Count)
                {
                    throw new Exception();
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(FormatRightWidth(Values[0], valueStringWidths[0]));
                for (int i = 1; i < ColumnNames.Count; i++)
                {
                    sb.Append(", " + FormatRightWidth(Values[i], valueStringWidths[i]));
                }
                return sb.ToString();
            }

            private static string FormatRightWidth(string value, int width)
            {
                string format = string.Format("{{0, -{0}}}", width);
                return string.Format(format, value);
            }
        }
    }
}
