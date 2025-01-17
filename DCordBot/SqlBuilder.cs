﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DCordBot
{
    class SqlBuilder : IDisposable
    {
        SqlCommand sqlCommand = null;
        public SqlBuilder(string commandName,CommandType commandType)
        {
            sqlCommand = new SqlCommand(commandName, Program.botConnection);
            sqlCommand.CommandType = commandType;
        }
        
        private void AddParameter(SqlParameter param)
        {
            sqlCommand.Parameters.Add(param);
        }

        public void AddParameter<T>(string paramName, SqlDbType sqlDbType, T value, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter parameter = new SqlParameter(paramName, value);
            parameter.SqlDbType = sqlDbType;
            parameter.Direction = direction;
            AddParameter(parameter);
        }

        public  async Task<long> ExecuteNonQueryAsync()
        {
            long result =  Convert.ToInt64(await sqlCommand.ExecuteNonQueryAsync());
            return result;
        }

        public async Task<SqlDataReader> ExecuteReader()
        {
            return await sqlCommand.ExecuteReaderAsync();
        }

        public long GetReturnValue(string returnParamname)
        {
            long val = Convert.ToInt64(sqlCommand.Parameters[returnParamname].Value);
            return val;
        }

        public object GetParameter(string name)
        {
            return sqlCommand.Parameters[name].Value;
        }

        public void Dispose()
        {
            sqlCommand.Dispose();
        }
    }
}
