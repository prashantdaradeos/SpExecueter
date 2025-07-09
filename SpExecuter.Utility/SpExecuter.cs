using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;



namespace SpExecuter.Utility
{

    public class SpExecutor
    {
        public async static ValueTask<List<ISpResponse>[]> ExecuteSpToObjects(string spName,
            string dbName = default, bool spNeedParameters = true, object spEntity = default,
            List<SqlParameter> param = default, int requestObjectNumber = 0, params int[] returnObjects)
        {

            if (spNeedParameters && param == null && spEntity != null)
            {
                param = GetParamFromObject(spEntity, requestObjectNumber);
            }
            List<ISpResponse>[] allTables = new List<ISpResponse>[returnObjects.Length == 0 ? 1 : returnObjects.Length ];
            GenericSpResponse genericDbResponse = new GenericSpResponse();
            string currentProp = "NA";
            try
            {
                using (SqlConnection connection = new SqlConnection(dbName))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddRange(param != null ? param.ToArray() : Array.Empty<SqlParameter>());
                        if (returnObjects.Length == 0)
                        {
                            genericDbResponse.NumberOfRowsAffected = await command.ExecuteNonQueryAsync();
                            allTables[0] = new List<ISpResponse>() { genericDbResponse };

                        }
                        else
                        {
                            using (SqlDataReader reader = await command.ExecuteReaderAsync())
                            {
                                for (int i = 0; i < returnObjects.Length; i++)
                                {

                                    int objectTypeIndex = returnObjects[i];
                                    if (objectTypeIndex == 0)
                                    {
                                        reader.NextResult();
                                        continue;
                                    }
                                    Type type = DBConstants.SpResponseModelTypeArray[objectTypeIndex];
                                    List<ISpResponse> list = new List<ISpResponse>();
                                    while (reader.Read())
                                    {
                                        ISpResponse dbResponseObject = (ISpResponse)Activator.CreateInstance(type);
                                        for (int j = 0; j < AppConstants.SpResponsePropertyInfoCache[objectTypeIndex].Length; j++)
                                        {
                                            PropertyInfo prop = AppConstants.SpResponsePropertyInfoCache[objectTypeIndex][j];
                                            bool isNotNull = !reader.IsDBNull(j);
                                            currentProp = prop.Name;
                                            if (isNotNull)
                                            {
                                              
                                                if (prop.PropertyType == AppConstants.StringType)
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetString(j));
                                                }
                                                else if (prop.PropertyType == AppConstants.IntType)
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetInt32(j));
                                                }
                                                else if (prop.PropertyType == AppConstants.BoolType)
                                                {
                                                    prop.SetValue(dbResponseObject, Convert.ToBoolean(reader.GetValue(j)));
                                                }
                                                else if (prop.PropertyType == AppConstants.LongType)
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetInt64(j));
                                                }
                                                else if (prop.PropertyType == AppConstants.DateTimeType)
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetDateTime(j));
                                                }
                                                else if (prop.PropertyType == AppConstants.DoubleType)
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetDouble(j));
                                                }
                                                else if (prop.PropertyType == AppConstants.FloatType)
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetFloat(j));
                                                }
                                                else if (prop.PropertyType == AppConstants.ByteArrayType)
                                                {
                                                    long length = reader.GetBytes(j, 0, null, 0, 0);
                                                    byte[] buffer = new byte[length];
                                                    reader.GetBytes(j, 0, buffer, 0, (int)length);
                                                    prop.SetValue(dbResponseObject, buffer);
                                                }



                                            }
                                        }
                                        list.Add(dbResponseObject);
                                    }
                                    allTables[i] = list;
                                    reader.NextResult();
                                }

                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                StringBuilder info = new StringBuilder().AppendLine("Stored Procedure --> " + spName)
                    .Append("  :::  ")
                    .AppendLine("Current Property --> " + currentProp)
                    .Append("  :::  ")
                    .AppendLine("DataBase --> " + dbName)
                    .Append("  :::  Return classes --> ");
                foreach (int objectTypeIndex in returnObjects) {
                    info.Append(DBConstants.SpResponseModelTypeArray[objectTypeIndex].Name+", ");
                }
                    info.AppendLine("")
                    .AppendLine("  :::  Parameter Object --> " + JsonSerializer.ToJsonString(spEntity))
                    .AppendLine("  :::  "+ "SQL Parameters --> " );
                foreach (var oneparam in param)
                {
                    info.Append("       "+oneparam.ParameterName+" : ").AppendLine(oneparam.Value+",  ");
                }

                throw new SpExecuterException(info, ex);

            }
           
            return allTables;
        }
       
        public static List<SqlParameter> GetParamFromObject(object obj, int requestObjectNumber)
        {
            var paramList = new List<SqlParameter>();
            var spParameters = AppConstants.SpRequestPropertyInfoCache[requestObjectNumber];
            var accessors = AppConstants.CachedPropertyAccessorDelegates[requestObjectNumber];

            foreach (var parameter in spParameters)
            {
                // 1) Get the raw value (object)
                object rawValue = accessors[parameter.Name](obj);

                SqlParameter param;
                var propType = parameter.PropertyType;
                if (propType == AppConstants.ByteArrayType)
                {
                    var bytes = rawValue as byte[];
                    param = new SqlParameter(
                         "@" + parameter.Name,
                        SqlDbType.VarBinary,
                         -1  // -1 means VARBINARY(MAX)
                    )
                    {
                        Value = (object)bytes ?? DBNull.Value
                    };
                }
                else if (propType == AppConstants.DateTimeType || propType == AppConstants.DateTimeNullableType)
                {
                    var dt = rawValue as DateTime?;
                    param = new SqlParameter("@" + parameter.Name, SqlDbType.DateTime)
                    {
                        Value = dt.HasValue ? dt.Value : DBNull.Value
                    };
                }
                else if (propType == AppConstants.DateTimeOffsetType || propType == AppConstants.NullableDateTimeOffsetType)
                {
                    var dto = rawValue as DateTimeOffset?;
                    param = new SqlParameter("@" + parameter.Name, SqlDbType.DateTimeOffset)
                    {
                        Value = dto.HasValue ? dto.Value : DBNull.Value
                    };
                }
                else if (propType == AppConstants.TimeSpanType || propType == AppConstants.NullableTimeSpanType)
                {
                    var ts = rawValue as TimeSpan?;
                    param = new SqlParameter("@" + parameter.Name, SqlDbType.Time)
                    {
                        Value = ts.HasValue ? ts.Value : DBNull.Value
                    };
                }
                else if (propType.IsGenericType
                    && (propType.GetGenericTypeDefinition() == AppConstants.ListType ||
                    propType.GetGenericTypeDefinition() == AppConstants.IListType))
                {
                    Type elementType = propType.GetGenericArguments()[0];
                    var list = rawValue as System.Collections.IList;
                    Delegate del = DBConstants.tVPsdelegates[elementType.Name];
                    DataTable dt=((Func<System.Collections.IList,DataTable>)del)(list);
                    // You can iterate it, or convert to DataTable, etc.
                     param = new SqlParameter("@" + parameter.Name, SqlDbType.Structured)
                    {
                        TypeName = "dbo."+elementType.Name, // SQL user-defined table type name
                        Value = (object)dt ?? DBNull.Value
                    };
                }
                else
                {
                    string str = Convert.ToString(rawValue);

                    object val = string.IsNullOrWhiteSpace(str)
                        ? DBNull.Value
                        : str;

                    param = new SqlParameter("@" + parameter.Name, val);
                }

                paramList.Add(param);
            }

            return paramList;
        }
        public async static ValueTask<List<List<string[]>>> ExecuteSpToStringArray(
              string spName, string connectionString,
              int requestObjectNumber = 0, object spEntity = null!,
              List<SqlParameter> param = null,
              int[][] indexesArray = null, int[] takeProperties = null)
        {
            // (Optional) build parameters from spEntity if needed
            if ((param == null || param.Count == 0) && spEntity != null)
            {
                param = GetParamFromObject(spEntity, requestObjectNumber);
            }

            var allResultSets = new List<List<string[]>>();
            int tableIndex = 0;  // track which result set we are on

            using (var connection = new SqlConnection(connectionString))
            {
                 connection.Open();
                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (param != null)
                    {
                        command.Parameters.AddRange(param.ToArray());
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Loop over each result set (table)
                        do
                        {
                            int numberOfProperties;
                            int[] currentIndexArray;

                            // 1. If takeProperties specifies a count for this table, use first N columns
                            if (takeProperties != null && tableIndex < takeProperties.Length)
                            {
                                numberOfProperties = takeProperties[tableIndex];
                                currentIndexArray = Enumerable.Range(0, numberOfProperties).ToArray();
                            }
                            // 2. Else if indexesArray specifies indexes for this table, use those indexes
                            else if (indexesArray != null && tableIndex < indexesArray.Length)
                            {
                                currentIndexArray = indexesArray[tableIndex];
                                numberOfProperties = currentIndexArray.Length;
                            }
                            // 3. Otherwise, take all columns by default
                            else
                            {
                                numberOfProperties = reader.FieldCount;
                                currentIndexArray = Enumerable.Range(0, numberOfProperties).ToArray();
                            }

                            // Read all rows in this result set
                            var rows = new List<string[]>();
                            while (reader.Read())
                            {
                                string[] rowValues = new string[numberOfProperties];
                                for (int i = 0; i < currentIndexArray.Length; i++)
                                {
                                    int colIndex = currentIndexArray[i];
                                    if (!reader.IsDBNull(colIndex))
                                    {
                                        rowValues[i] = reader.GetValue(colIndex).ToString()!;
                                    }
                                    else
                                    {
                                        rowValues[i] = string.Empty;
                                    }
                                }
                                rows.Add(rowValues);
                            }
                            allResultSets.Add(rows);

                            tableIndex++;
                        }
                        // Move to next result set (table), if any
                        while (reader.NextResult());
                    }
                }
            }

            return allResultSets;
        }


        public static List<T> GetStronglyTypedList<T>(List<ISpResponse> raw)
            {
                List<T> result = new List<T>(raw.Count);
                for (int i = 0; i < raw.Count; i++)
                {
                    result.Add((T)raw[i]);
                }
                return result;
            }
        

    }

}
