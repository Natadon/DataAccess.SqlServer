using Natadon.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Natadon.DataAccess.SqlServer
{
    public class SqlServerDataAccess : IDataAccess
    {
        private readonly string connectionString;
        private SqlCommand command;
        private SqlConnection connection;
        private SqlDataAdapter dataAdapter;

        public SqlServerDataAccess(string ServerName, string DatabaseName, string UserName = "", string Password = "")
        {
            if(UserName == "")
            {
                connectionString = string.Format("Server={0};Database={1};Trusted_Connection=True;", ServerName, DatabaseName);
            }
            else
            {
                connectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};", ServerName, DatabaseName, UserName, Password);
            }
        }

        public void ExecuteQuery(string SqlStatement, List<DataAccessParameter> Params = null)
        {
            connection.Open();

            try
            {
                command = new SqlCommand(SqlStatement, connection);

                if(Params != null)
                {
                    List<SqlParameter> sqlParameters = getParams(Params);

                    command.Parameters.AddRange(sqlParameters.ToArray());
                }

                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();

                connection.Close();
            }
        }

        public DataSet GetDataSet(string SqlStatement, List<DataAccessParameter> Params = null)
        {
            connection.Open();

            try
            {
                command = new SqlCommand(SqlStatement, connection);

                if (Params != null)
                {
                    List<SqlParameter> sqlParameters = getParams(Params);

                    command.Parameters.AddRange(sqlParameters.ToArray());
                }

                dataAdapter = new SqlDataAdapter(command);
                DataSet ds = new DataSet();

                dataAdapter.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();

                connection.Close();
            }
        }

        public object GetScalar(string SqlStatement, List<DataAccessParameter> Params = null)
        {
            connection.Open();

            try
            {
                command = new SqlCommand(SqlStatement, connection);

                if(Params != null)
                {
                    List<SqlParameter> sqlParameters = getParams(Params);

                    command.Parameters.AddRange(sqlParameters.ToArray());
                }

                object obj = command.ExecuteScalar();

                return obj;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        private List<SqlParameter> getParams(List<DataAccessParameter> accessParameters)
        {
            List<SqlParameter> retVal = new List<SqlParameter>();

            foreach(var item in accessParameters)
            {
                retVal.Add(new SqlParameter(item.ParameterName, item.Value));
            }

            return retVal;
        }
    }
}
