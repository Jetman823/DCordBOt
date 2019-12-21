using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DCordBot
{
    class SqlBuilder
    {
        SqlCommand sqlCommand = null;
        public SqlBuilder(SqlCommand command)
        {
            sqlCommand = command;
        }
        
        public void AddParameter(SqlParameter param)
        {
            sqlCommand.Parameters.Add(param);
        }

        public void AddParameter(string paramName, SqlDbType sqlDbType, long value, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter parameter = new SqlParameter(paramName, value);
            parameter.SqlDbType = sqlDbType;
            parameter.Direction = direction;
            AddParameter(parameter);
        }

        public async Task<long> ExecuteStoredProcedure()
        {
            if (sqlCommand.CommandType != CommandType.StoredProcedure)
                return -1;
            SqlDataReader reader = null;
            try
            {
               reader =  await sqlCommand.ExecuteReaderAsync();
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
            long result = 0;
            try
            {
                while (reader.Read())
                {
                    result = reader.GetInt64(0);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                result = -1;
                reader.Close();
                return result;
            }
            reader.Close();
            return result;
        }
    }
}
