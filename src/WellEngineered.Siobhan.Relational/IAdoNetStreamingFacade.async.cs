/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace WellEngineered.Siobhan.Relational
{
	internal partial interface IAdoNetStreamingFacade
	{
		#region Methods/Operators

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An asynchronous enumerable of task-wrapped result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		IAsyncEnumerable<IAdoNetStreamingRecord> ExecuteRecordsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of results, each with an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An asynchronous enumerable of task-wrapped result instances, each containing an enumerable of dictionaries with record key/value pairs of data. </returns>
		IAsyncEnumerable<IAsyncAdoNetStreamingResult> ExecuteResultsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An asynchronous enumerable of task-wrapped result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		IAsyncEnumerable<IAdoNetStreamingRecord> ExecuteSchemaRecordsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an results, each with an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An asynchronous enumerable of task-wrapped result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		IAsyncEnumerable<IAsyncAdoNetStreamingResult> ExecuteSchemaResultsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An asynchronous enumerable of task-wrapped record dictionary instances, containing key/value pairs of data. </returns>
		IAsyncEnumerable<IAdoNetStreamingRecord> GetRecordsFromReaderAsync(DbDataReader dbDataReader, Action<int> recordsAffectedCallback, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of results, each with an enumerable of records.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <returns> An asynchronous enumerable of task-wrapped result instances, each containing an enumerable of dictionaries with record key/value pairs of data. </returns>
		IAsyncEnumerable<IAsyncAdoNetStreamingResult> GetResultsFromReaderAsync(DbDataReader dbDataReader, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An asynchronous enumerable of task-wrapped record dictionary instances, containing key/value pairs of schema metadata. </returns>
		IAsyncEnumerable<IAdoNetStreamingRecord> GetSchemaRecordsFromReaderAsync(DbDataReader dbDataReader, Action<int> recordsAffectedCallback, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of results, each with an enumerable of records.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method performs ASYNC/TASK EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <returns> An asynchronous enumerable of task-wrapped result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		IAsyncEnumerable<IAsyncAdoNetStreamingResult> GetSchemaResultsFromReaderAsync(DbDataReader dbDataReader, CancellationToken cancellationToken = default);

		#endregion
	}
}
#endif