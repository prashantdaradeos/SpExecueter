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
        public async static Task<List<ISpResponse>[]> ExecuteSpToObjects(string spName,
            string dbName = default, bool spNeedParameters = true, object spEntity = default,
            List<SqlParameter> param = default, int requestObjectNumber = 0, params int[] returnObjects)
        {

            if (spNeedParameters && param == null && spEntity != null)
            {
                param = GetParamFromObject(spEntity, requestObjectNumber);
            }
            List<ISpResponse>[] allTables = new List<ISpResponse>[returnObjects.Length == 0 ? 1 : returnObjects.Length ];
            GenericSpResponse genericDbResponse = new GenericSpResponse();
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
                                            if (isNotNull)
                                            {
                                                if (prop.PropertyType == typeof(string))
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetString(j));
                                                }
                                                else if (prop.PropertyType == typeof(int))
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetInt32(j));
                                                }
                                                else if (prop.PropertyType == typeof(bool))
                                                {
                                                    prop.SetValue(dbResponseObject, Convert.ToBoolean(reader.GetValue(j)));
                                                }
                                                else if (prop.PropertyType == typeof(long))
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetInt64(j));
                                                }
                                                else if (prop.PropertyType == typeof(DateTime))
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetDateTime(j));
                                                }
                                                else if (prop.PropertyType == typeof(double))
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetDouble(j));
                                                }
                                                else if (prop.PropertyType == typeof(float))
                                                {
                                                    prop.SetValue(dbResponseObject, reader.GetFloat(j)); // reader.GetFloat = SQL REAL = .NET float
                                                }
                                                else if (prop.PropertyType == typeof(byte[]))
                                                {
                                                    long length = reader.GetBytes(j, 0, null, 0, 0); // get length
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
                StringBuilder info = new StringBuilder().Append(spName).Append("\r\n").Append(dbName).
                    Append("\r\n").Append(JsonSerializer.ToJsonString(returnObjects)).Append("\r\n")
                    .Append(JsonSerializer.ToJsonString(spEntity)).Append("\r\n");
                foreach (var oneparam in param)
                {
                    info.Append(oneparam.ParameterName).Append(oneparam.Value);
                }

                throw new SpExecuterException(info, ex);

            }
           
            return allTables;
        }
        /*public static List<SqlParameter> GetParamFromObject(object obj, int requestObjectNumber)
        {
            List<SqlParameter> paramList = new List<SqlParameter>();
            PropertyInfo[] spParameters = AppConstants.SpRequestPropertyInfoCache[requestObjectNumber];
            SqlParameter param;
            foreach (var parameter in spParameters)
            {
                string value = Convert.ToString(AppConstants.CachedPropertyAccessorDelegates[requestObjectNumber][parameter.Name](obj));
                object val;
                if (string.IsNullOrWhiteSpace(value))
                {
                    val = value;
                }
                else
                {
                    val = DBNull.Value;
                }
                param = new SqlParameter($"{AppConstants.AtTheRate}{parameter.Name}", val);
                paramList.Add(param);
            }
            return paramList;
        }*/
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
                if (propType == typeof(byte[]))
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
                else if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                {
                    var dt = rawValue as DateTime?;
                    param = new SqlParameter("@" + parameter.Name, SqlDbType.DateTime)
                    {
                        Value = dt.HasValue ? dt.Value : DBNull.Value
                    };
                }
                else if (propType == typeof(DateTimeOffset) || propType == typeof(DateTimeOffset?))
                {
                    var dto = rawValue as DateTimeOffset?;
                    param = new SqlParameter("@" + parameter.Name, SqlDbType.DateTimeOffset)
                    {
                        Value = dto.HasValue ? dto.Value : DBNull.Value
                    };
                }
                else if (propType == typeof(TimeSpan) || propType == typeof(TimeSpan?))
                {
                    var ts = rawValue as TimeSpan?;
                    param = new SqlParameter("@" + parameter.Name, SqlDbType.Time)
                    {
                        Value = ts.HasValue ? ts.Value : DBNull.Value
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
