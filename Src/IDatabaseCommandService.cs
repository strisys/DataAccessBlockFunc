// Copyright (c) Strisys Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Transactions;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace DataAccessFoundation {
    public interface IDatabaseCommandService {
        /// <summary>
        /// Gets the name of the environment.
        /// </summary>
        /// <value>
        /// The name of the environment.
        /// </value>
        String EnvironmentName { get; }

        /// <summary>
        /// Gets the name of the user executing the commands.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        String UserName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to suppress logging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if suppress logging; otherwise, <c>false</c>.
        /// </value>
        Boolean SuppressLogging { get; set; }

        /// <summary>
        /// Gets the default database schema.
        /// </summary>
        /// <value>
        /// The default database schema.
        /// </value>
        String SchemaName { get; }

        /// <summary>
        /// Gets the connection string from the configured environment.
        /// </summary>
        /// <returns></returns>
        String GetConnectionString();

        /// <summary>
        /// Gets the Enterprise Library database.
        /// </summary>
        /// <returns></returns>
        Database GetDataBase();

        /// <summary>
        /// Gets the <see cref="TransactionScope"/>.
        /// </summary>
        /// <param name="option">The <see cref="TransactionScopeOption"/>.</param>
        /// <returns><see cref="TransactionScope"/></returns>
        /// <code>
        ///     using(TransactionScope scope = GetTransactionScope()) {
        ///        // execution commands
        ///     }
        /// </code>
        TransactionScope GetTransactionScope(TransactionScopeOption option = TransactionScopeOption.Required);

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns><see cref="Database"/></returns>
        Database CreateDatabase(String connectionString);

        /// <summary>
        /// Executes a command and returns the number of rows affected.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns>Return code</returns>
        Int32 ExecuteSpNonQuery(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null);

        /// <summary>
        /// Executes a command and returns a value.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns>Scalar value</returns>
        Object ExecuteSpScalar(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null);

        /// <summary>
        /// Executes a command and returns a <see cref="DataSet" />
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns><see cref="DataSet" /></returns>
        DataSet ExecuteSpDataSet(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null);

        /// <summary>
        /// Executes a command and returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns><see cref="DataTable" /></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        DataTable ExecuteSpDataTable(String storedProcedureName, Action<DatabaseParameterCollection> parametersFunc = null);

        /// <summary>
        /// Executes the SQL <see cref="DataSet"/>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns><see cref="DataSet"/></returns>
        DataSet ExecuteSqlDataSet(String sql);

        /// <summary>
        /// Executes the SQL to return a <see cref="DataSet"/>
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parametersFunc">The parameters function.</param>
        /// <returns><see cref="DataSet"/></returns>
        DataSet ExecuteSqlDataSet(String sql, Action<DatabaseParameterCollection> parametersFunc);

        /// <summary>
        /// Executes the <b>SQL</b> and returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <returns><see cref="DataTable"/></returns>
        DataTable ExecuteSqlDataTable(String sql);

        /// <summary>
        /// Executes the <b>SQL</b> and returns a <see cref="DataTable" />
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns><see cref="DataTable"/></returns>
        DataTable ExecuteSqlDataTable(String sql, Action<DatabaseParameterCollection> parametersFunc);

        /// <summary>
        /// Executes a command and returns a value.
        /// </summary>
        /// <param name="sql">Sql to be run.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns>Scalar value</returns>
        Object ExecuteSqlScalar(String sql, Action<DatabaseParameterCollection> parametersFunc = null);

        /// <summary>
        /// Executes a sql statement and returns number of rows affected.
        /// </summary>
        /// <param name="sql">Sql to be run.</param>
        /// <param name="parametersFunc">The callback used to specify parameters.</param>
        /// <returns>Return code</returns>
        Int32 ExecuteSqlNonQuery(String sql, Action<DatabaseParameterCollection> parametersFunc = null);

        /// <summary>
        /// Executes the specified execute parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executeParams">The execute parameters.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        T Execute<T>(Tuple<Database, DbCommand, DatabaseParameterCollection> executeParams, Func<T> func);

        /// <summary>
        /// Coverts the specified list of <see cref="Int32"/> the delimited string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns><see cref="String"/> of delimited values</returns>
        String ToDelimitedString(IEnumerable<Int32> values, String delimiter = ", ");

        /// <summary>
        /// Coverts the specified list of <see cref="Int64"/> the delimited string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns><see cref="String"/> of delimited values</returns>
        String ToDelimitedString(IEnumerable<Int64> values, String delimiter = ", ");
    }
}