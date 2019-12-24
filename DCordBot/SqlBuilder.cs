using System;
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

        public async Task<long> ExecuteStoredProcedure()
        {
            return 0;
        }

        public  async Task<int> ExecuteNonQueryAsync()
        {
            int result =  await sqlCommand.ExecuteNonQueryAsync();
            return result;
        }

        public long GetReturnValue(string returnParamname)
        {
            return Convert.ToInt64(sqlCommand.Parameters[returnParamname].Value);
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
