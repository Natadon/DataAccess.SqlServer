using Natadon.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Natadon.DataAccess.SqlServer
{
    public class SqlServerDataAccess : IDataAccess
    {
        // The connection string for this connection
        private readonly string connectionString;

        // the SqlCommand used to connection to the database
        private SqlCommand command;

        // The connection to the server
        private SqlConnection connection;

        // The data adapter to conver the results to System.Data.DataSets
        private SqlDataAdapter dataAdapter;

        // The command timeout (in seconds)
        int commandTimeout = 30;

        /// <summary>
        /// Create an instance of the DataAccess object
        /// </summary>
        /// <param name="ServerName">The name (or IP) of the server to connect to</param>
        /// <param name="DatabaseName">The name of the database to use</param>
        /// <param name="UserName">The optional username to use to authenticate to the server</param>
        /// <param name="Password">The optional password to use to authenticate to the server</param>
        /// <param name="CommandTimeOut">The timeout period (in seconds) for the commands to run</param>
        public SqlServerDataAccess(string ServerName, string DatabaseName, string UserName = "", string Password = "", int CommandTimeOut = 30)
        {
            if(UserName == "")
            {
                connectionString = string.Format("Server={0};Database={1};Trusted_Connection=True;", ServerName, DatabaseName);
            }
            else
            {
                connectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};", ServerName, DatabaseName, UserName, Password);
            }

            connection = new SqlConnection(connectionString);
            commandTimeout = CommandTimeOut;
        }

        public void ExecuteQuery(string SqlStatement, List<DataAccessParameter> Params = null)
        {
            connection.Open();

            try
            {
                command = new SqlCommand(SqlStatement, connection);
                command.CommandTimeout = commandTimeout;

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
                command.CommandTimeout = commandTimeout;

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
                command.CommandTimeout = commandTimeout;

                if (Params != null)
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

        /// <summary>
        /// Converts the DataAccessParameters to SqlParameters
        /// </summary>
        /// <param name="accessParameters"></param>
        /// <returns></returns>
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
