// Copyright (c) Strisys Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using NLog;

namespace DataAccessFoundation {
    /// <summary>
    /// Service for issuing data access command using Microsoft Data Access Blocks
    /// </summary>
    public class DatabaseCommandService : IDatabaseCommandService {
        #region Fields

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static readonly IDictionary<String, Database> _databases = new ConcurrentDictionary<String, Database>();
        private readonly String _connectionString, _environmentName, _userName, _schemaName;
        private Database _dataBase;
        private Boolean _supressLogging;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseCommandService" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="environmentName">Name of the environment.</param>
        /// <param name="userName">Name of the user.</param>
        public DatabaseCommandService(String connectionString, String schemaName = "dbo", String environmentName = null, String userName = null) {
            _connectionString = connectionString;
            _schemaName = (schemaName ?? "dbo");
            _environmentName = (environmentName ?? "null");
            _userName = (userName ?? "null");
            _supressLogging = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ILogger"/>.
        /// </summary>
        public virtual ILogger Logger {
            get { return _logger; }
        }

        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        /// <value>
        /// The name of the environment.
        /// </value>
        public virtual String EnvironmentName {
            get { return (_environmentName ?? "null"); }
        }
        
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public virtual String UserName {
            get { return (_userName ?? "null"); }
        }

        /// <summary>
        /// Gets the connection string from the configured environment.
        /// </summary>
        /// <returns></returns>
        public virtual String GetConnectionString() {
            return _connectionString;
        }

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        /// <value>
        /// The name of the schema.
        /// </value>
        public virtual String SchemaName {
            get { return _schemaName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether suppress logging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if suppress logging; otherwise, <c>false</c>.
        /// </value>
        public virtual Boolean SuppressLogging {
            get { return _supressLogging; }
            set { _supressLogging = value; }
        }

        /// <summary>
        /// Gets the Enterprise Library database.
        /// </summary>
        /// <returns></returns>
        public virtual Database GetDataBase() {
            String connString = GetConnectionString();
            Database database;

            if (_databases.TryGetValue(connString, out database)) {
                return database;
            }

            return (_databases[connString] = CreateDatabase(connString));
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public virtual Database CreateDatabase(String connectionString) {
            return (_dataBase ?? (_dataBase = new SqlDatabase(connectionString)));
        }

        #endregion

        #region ExecuteSpNonQuery

        /// <summary>
        /// Executes a command and returns the number of rows affected.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns></returns>
        public virtual Int32 ExecuteSpNonQuery(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null) {
            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = null;

            try {
                executeParams = SetUpCommand(storedProcedureName, CommandType.StoredProcedure, parametersFunc);
                Func<Int32> func = () => executeParams.Item1.ExecuteNonQuery(executeParams.Item2);
                return Execute(executeParams, func);
            }
            catch (Exception ex) {
                Logger.Error(ex, String.Format("Failed to execute the stored procedure [{0}, Parameters:={1}]. [Environment:={2}]  {3}", storedProcedureName, GetParametersDescription(executeParams), EnvironmentName, ex.Message));
                throw;
            }
        }

        #endregion

        #region ExecuteSpScalar

        /// <summary>
        /// Executes a command and returns a value.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns></returns>
        public virtual Object ExecuteSpScalar(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null) {
            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = null;

            try {
                executeParams = SetUpCommand(storedProcedureName, CommandType.StoredProcedure, parametersFunc);
                Func<Object> func = () => executeParams.Item1.ExecuteScalar(executeParams.Item2);
                return Execute(executeParams, func);
            }
            catch (Exception ex) {
                Logger.Error(ex, String.Format("Failed to execute the stored procedure [{0}, Parameters:={1}]. [Environment:={2}]  {3}", storedProcedureName, GetParametersDescription(executeParams), EnvironmentName, ex.Message));
                throw;
            }
        }

        #endregion

        #region ExecuteSpDataSet

        /// <summary>
        /// Executes a command and returns a <see cref="DataSet" />
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns>
        ///   <see cref="DataSet" />
        /// </returns>
        public virtual DataSet ExecuteSpDataSet(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null) {
            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = null;

            try {
                executeParams = SetUpCommand(storedProcedureName, CommandType.StoredProcedure, parametersFunc);
                Func<DataSet> func = () => executeParams.Item1.ExecuteDataSet(executeParams.Item2);
                return Execute(executeParams, func);
            }
            catch (Exception ex) {
                Logger.Error(ex, String.Format("Failed to execute the stored procedure [{0}, Parameters:={1}]. [Environment:={2}]  {3}", storedProcedureName, GetParametersDescription(executeParams), EnvironmentName, ex.Message));
                throw;
            }
        }

        #endregion

        #region ExecuteSpDataTable

        /// <summary>
        /// Executes a command and returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns>
        ///   <see cref="DataTable" />
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public virtual DataTable ExecuteSpDataTable(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null) {
            DataSet dataSet = ExecuteSpDataSet(storedProcedureName, parametersFunc);

            if ((null != dataSet) && (dataSet.Tables.Count > 0)) {
                DataTable dataTable = dataSet.Tables[0];

                if (String.IsNullOrEmpty(dataTable.TableName)) {
                    dataTable.TableName = storedProcedureName;
                }

                return dataTable;
            }

            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = SetUpCommand(storedProcedureName, CommandType.StoredProcedure, parametersFunc);
            throw new InvalidOperationException(String.Format("Failed to execute the stored procedure [{0}, Parameters:={1}].  No table(s) returned. [Environment:={2}]", storedProcedureName, GetParametersDescription(executeParams), EnvironmentName));
        }

        #endregion

        #region ExecuteSqlDataSet

        /// <summary>
        /// Executes the SQL <see cref="DataSet"/>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public virtual DataSet ExecuteSqlDataSet(String sql) {
            return ExecuteSqlDataSet(sql, null);
        }

        /// <summary>
        /// Executes the SQL to return a <see cref="DataSet"/>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parametersFunc">The parameters function.</param>
        /// <returns></returns>
        public virtual DataSet ExecuteSqlDataSet(String sql, Action<DatabaseParameterCollection> parametersFunc) {
            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = null;

            try {
                executeParams = SetUpCommand(sql, CommandType.Text, parametersFunc);
                Func<DataSet> func = (() => executeParams.Item1.ExecuteDataSet(executeParams.Item2));
                return Execute(executeParams, func);
            }
            catch (Exception ex) {
                Logger.Error(ex, String.Format("Failed to execute the sql [{0}, Parameters:={1}]. [Environment:={2}]  {3}", sql, GetParametersDescription(executeParams), EnvironmentName, ex.Message));
                throw;
            }
        }

        #endregion

        #region ExecuteSqlDataTable

        /// <summary>
        /// Executes the <b>SQL</b> and returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <returns></returns>
        public virtual DataTable ExecuteSqlDataTable(String sql) {
            return ExecuteSqlDataTable(sql, null);
        }

        /// <summary>
        /// Executes the <b>SQL</b> and returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns></returns>
        public virtual DataTable ExecuteSqlDataTable(String sql, Action<DatabaseParameterCollection> parametersFunc) {
            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = null;

            try {
                executeParams = SetUpCommand(sql, CommandType.Text, parametersFunc);
                Func<DataTable> func = (() => executeParams.Item1.ExecuteDataSet(executeParams.Item2).Tables[0]);
                return Execute(executeParams, func);
            }
            catch (Exception ex) {
                Logger.Error(ex, String.Format("Failed to execute the sql [{0}, Parameters:={1}]. [Environment:={2}]  {3}", sql, GetParametersDescription(executeParams), EnvironmentName, ex.Message));
                throw;
            }
        }

        #endregion

        #region ExecuteSqlScalar

        /// <summary>
        /// Executes a command and returns a value.
        /// </summary>
        /// <param name="sql">Sql to be run.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns></returns>
        public virtual Object ExecuteSqlScalar(String sql, Action<DatabaseParameterCollection> parametersFunc = null) {
            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = null;

            try {
                executeParams = SetUpCommand(sql, CommandType.Text, parametersFunc);
                Func<Object> func = () => executeParams.Item1.ExecuteScalar(executeParams.Item2);
                return Execute(executeParams, func);
            }
            catch (Exception ex) {
                Logger.Error(ex, String.Format("Failed to execute sql scalar [{0}, Parameters:={1}]. [Environment:={2}]  {3}", sql, GetParametersDescription(executeParams), EnvironmentName, ex.Message));
                throw;
            }
        }

        #endregion

        #region ExecuteSqlNonQuery

        /// <summary>
        /// Executes a sql statement and returns number of rows affected.
        /// </summary>
        /// <param name="sql">Sql to be run.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns>Number of rows affected</returns>
        public virtual int ExecuteSqlNonQuery(String sql, Action<DatabaseParameterCollection> parametersFunc = null) {
            Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams = null;

            try {
                executeParams = SetUpCommand(sql, CommandType.Text, parametersFunc);
                Func<Object> func = () => executeParams.Item1.ExecuteNonQuery(executeParams.Item2);
                return (int)Execute(executeParams, func);
            }
            catch (Exception ex) {
                Logger.Error(ex, String.Format("Failed to execute sql scalar [{0}, Parameters:={1}]. [Environment:={2}]  {3}", sql, GetParametersDescription(executeParams), EnvironmentName, ex.Message));
                throw;
            }
        }

        #endregion

        #region Db Interaction Utility

        public static String GetParametersDescription(Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams) {
            if (null == executeParams) {
                return "None";
            }

            DatabaseParameterCollection parameters = executeParams.Item3;

            if (null == parameters) {
                return "None";
            }

            return String.Format("Parameter Summary [Timeout:={0}]{1}{2}{1}{3}", parameters.Timeout, System.Environment.NewLine, (new String('-', 80)), parameters);
        }

        public virtual T Execute<T>(Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams, Func<T> func) {
            String timestamp = DateTime.Now.ToString("hh:mm:ss:ffff tt");
            ILogger logger = Logger;

            // Execute
            if (logger.IsDebugEnabled && (false == SuppressLogging)) {
                logger.Debug("Executing the command [{0}]] ... [User:={1}, Environment:={2}, Timestamp:={3}]", executeParams.Item2.CommandText, UserName, EnvironmentName, timestamp);
            }

            Int32 start = GetTickCount();
            T val = func();

            if (logger.IsDebugEnabled && (false == SuppressLogging)) {
                logger.Debug("Executed the command [{0}] successfully in {1} milliseconds! [User:={2}, Environment:={3}, Timestamp:={4}]", executeParams.Item2.CommandText, (GetTickCount() - start), UserName, EnvironmentName, timestamp);
            }

            // Get output parameters
            foreach (IDbDataParameter parameter in executeParams.Item2.Parameters) {
                if ((parameter.Direction != ParameterDirection.Output) && (parameter.Direction != ParameterDirection.InputOutput) && (parameter.Direction != ParameterDirection.ReturnValue)) {
                    continue;
                }

                DatabaseParameter dbParam = executeParams.Item3[parameter.ParameterName];
                dbParam.Value = parameter.Value;
            }

            return val;
        }

        public virtual Tuple<Database, DbCommand, DatabaseParameterCollection> SetUpCommand(String text, CommandType type, Action<DatabaseParameterCollection> parametersFunc = null) {
            if (String.IsNullOrWhiteSpace(text)) {
                throw new ArgumentException("text");
            }

            if (CommandType.StoredProcedure == type) {
                if (false == text.Contains(".")) {
                    text = String.Format("{0}.{1}", SchemaName, text);
                }
            }

            DatabaseParameterCollection parameters = SetUpParameters(parametersFunc);
            Database db = GetDataBase();
            DbCommand cmd = null;

            if (CommandType.StoredProcedure == type) {
                cmd = db.GetStoredProcCommand(text);
            }

            if (CommandType.Text == type) {
                cmd = db.GetSqlStringCommand(text);
            }

            if ((null != parameters) && (null != cmd) && (parameters.Timeout > 0)) {
                cmd.CommandTimeout = parameters.Timeout;
            }

            if ((null == parameters) || (0 == parameters.Count)) {
                return (new Tuple<Database, DbCommand, DatabaseParameterCollection>(db, cmd, parameters));
            }

            foreach (DatabaseParameter parameter in parameters) {
                if (parameter.Direction == ParameterDirection.Output) {
                    db.AddOutParameter(cmd, parameter.Name, parameter.Type, parameter.Size);
                    continue;
                }

                if (parameter.Direction == ParameterDirection.InputOutput) {
                    db.AddParameter(cmd, parameter.Name, parameter.Type, parameter.Size, parameter.Direction, false, 0, 0, String.Empty, DataRowVersion.Current, parameter.Value);
                    continue;
                }

                db.AddInParameter(cmd, parameter.Name, parameter.Type, parameter.Value);
            }

            return (new Tuple<Database, DbCommand, DatabaseParameterCollection>(db, cmd, parameters));
        }

        /// <summary>
        /// Sets up parameters.
        /// </summary>
        /// <param name="parametersFunc">The parameters function.</param>
        /// <returns></returns>
        public virtual DatabaseParameterCollection SetUpParameters(Action<DatabaseParameterCollection> parametersFunc = null) {
            DatabaseParameterCollection parameters = ((null != parametersFunc) ? (new DatabaseParameterCollection()) : null);

            if (null != parametersFunc) {
                parametersFunc(parameters);
            }

            return parameters;
        }

        /// <summary>
        /// Gets the tick count.
        /// </summary>
        /// <returns></returns>
        public Int32 GetTickCount() {
            return Environment.TickCount;
        }

        #endregion

        #region Utility

        /// <summary>
        /// Coverts the specified list of <see cref="Int32"/> the delimited string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns><see cref="String"/> of delimited values</returns>
        public virtual String ToDelimitedString(IEnumerable<Int32> values, String delimiter = ", ") {
            IList<Int64> vals = values.Select((s => (Int64)s)).ToArray();
            return ToDelimitedString(vals, delimiter);
        }

        /// <summary>
        /// Coverts the specified list of <see cref="Int64"/> the delimited string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns><see cref="String"/> of delimited values</returns>
        public virtual String ToDelimitedString(IEnumerable<Int64> values, String delimiter = ", ") {
            if (null == values) {
                return String.Empty;
            }

            StringBuilder sb = new StringBuilder();
            IList<Int64> vals = ((values as IList<Int64>) ?? values.ToArray());

            for (Int32 i = 0; (i < vals.Count); i++) {
                sb.Append(vals[i].ToString());

                if ((i + 1) < vals.Count) {
                    sb.Append(delimiter);
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
