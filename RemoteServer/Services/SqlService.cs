﻿using RemoteServices.Models;
using RemoteServices.Services;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RemoteServer.Services
{
    internal class SqlService : MarshalByRefObject, ISqlService
    {
        private static string ConnectionString => Properties.Settings.Default.ConnectionString;
        private readonly ControlForm _activeControlForm;

        public SqlService()
        {
            _activeControlForm = ControlForm.GetInstance();
        }

        public SqlResult ExecuteCommand(string sqlCommand)
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            var result = new SqlResult();
            try
            {
                sqlConnection.Open();
                _activeControlForm.AppendToLog($"Command: {sqlCommand}");
                var command = new SqlCommand(sqlCommand, sqlConnection);
                var affectedRowsCount = command.ExecuteNonQuery();
                if (affectedRowsCount == -1) affectedRowsCount = 0;
                result.Message = $"Command executed successfully. Affected rows count is {affectedRowsCount}";
                sqlConnection.Close();
                result.IsSucceeded = true;
            }
            catch (SqlException exception)
            {
                result.Message = exception.Message;

                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                    _activeControlForm.AppendToLog($"Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                    _activeControlForm.AppendToLog($"Exception: {exception.Message}");
                }
            }
            _activeControlForm.AppendToLog("--------------");
            return result;
        }

        public SqlResult ExecuteQuery(string sqlQuery)
        {
            var sqlConnection = new SqlConnection(ConnectionString);
            var result = new SqlResult();
            try
            {
                sqlConnection.Open();
                _activeControlForm.AppendToLog($"Query: {sqlQuery}");
                var command = new SqlCommand(sqlQuery, sqlConnection);
                var sqlDataAdapter = new SqlDataAdapter(command);
                var dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet);
                result.Table = dataSet.Tables[0];

                sqlConnection.Close();
                result.IsSucceeded = true;
            }
            catch (SqlException exception)
            {
                result.Message = exception.Message;

                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                    _activeControlForm.AppendToLog($"Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                    _activeControlForm.AppendToLog($"Exception: {exception.Message}");
                }
            }
            _activeControlForm.AppendToLog("--------------");
            return result;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
