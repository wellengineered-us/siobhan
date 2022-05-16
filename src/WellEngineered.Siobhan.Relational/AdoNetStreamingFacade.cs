﻿/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using WellEngineered.Solder.Injection;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational
{
	public partial class AdoNetStreamingFacade
		: IAdoNetStreamingFacade
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the AdoNetStreamingFacade class.
		/// </summary>
		internal AdoNetStreamingFacade()
		{
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Create a new data parameter from the data source.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="sourceColumn"> Specifies the source column. </param>
		/// <param name="parameterDirection"> Specifies the parameter direction. </param>
		/// <param name="parameterDbType"> Specifies the parameter provider-(in)dependent type. </param>
		/// <param name="parameterSize"> Specifies the parameter size. </param>
		/// <param name="parameterPrecision"> Specifies the parameter precision. </param>
		/// <param name="parameterScale"> Specifies the parameter scale. </param>
		/// <param name="parameterNullable"> Specifies the parameter nullable-ness. </param>
		/// <param name="parameterName"> Specifies the parameter name. </param>
		/// <param name="parameterValue"> Specifies the parameter value. </param>
		/// <returns> The data parameter with the specified properties set. </returns>
		public DbParameter CreateParameter(DbConnection dbConnection, DbTransaction dbTransaction, string sourceColumn, ParameterDirection parameterDirection, DbType parameterDbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			DbParameter dbParameter;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			using (IDisposableDispatch<DbCommand> dbCommand = ResourceManager.__.Using(_, dbConnection.CreateCommand()))
				dbParameter = dbCommand.Target.CreateParameter();

			dbParameter.ParameterName = parameterName;
			dbParameter.Size = parameterSize;
			dbParameter.Value = parameterValue;
			dbParameter.Direction = parameterDirection;
			dbParameter.DbType = parameterDbType;
			dbParameter.IsNullable = parameterNullable;
			dbParameter.Precision = parameterPrecision;
			dbParameter.Scale = parameterScale;
			dbParameter.SourceColumn = sourceColumn;

			ResourceManager.__.Leave(_);

			return dbParameter;
		}

		/// <summary>
		/// Executes a command, returning a data reader, against a data source.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// This method DOES NOT DISPOSE OF DATA READER - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="commandBehavior"> The reader behavior. </param>
		/// <param name="commandTimeout"> The command timeout (use null for default). </param>
		/// <param name="commandPrepare"> Whether to prepare the command at the data source. </param>
		/// <returns> The data reader result. </returns>
		public AdoNetStreamingDataReader ExecuteReader(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, CommandBehavior commandBehavior, int? commandTimeout, bool commandPrepare)
		{
			AdoNetStreamingDataReader dbDataReader;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			using (IDisposableDispatch<DbCommand> dbCommand = ResourceManager.__.Using(_, dbConnection.CreateCommand()))
			{
				dbCommand.Target.Transaction = dbTransaction;
				dbCommand.Target.CommandType = commandType;
				dbCommand.Target.CommandText = commandText;

				if ((object)commandTimeout != null)
					dbCommand.Target.CommandTimeout = (int)commandTimeout;

				// add parameters
				if ((object)commandParameters != null)
				{
					foreach (DbParameter commandParameter in commandParameters)
					{
						if ((object)commandParameter.Value == null)
							commandParameter.Value = DBNull.Value;

						dbCommand.Target.Parameters.Add(commandParameter);
					}
				}

				if (commandPrepare)
					dbCommand.Target.Prepare();

				// do the database work
				var dataReader = dbCommand.Target.ExecuteReader(commandBehavior);
				dbDataReader = new AdoNetStreamingDataReader(dataReader);

				// clean out parameters
				//dbCommand.Parameters.Clear();
			}

			ResourceManager.__.Leave(_);

			return dbDataReader;
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IAdoNetStreamingRecord> ExecuteRecords(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IAdoNetStreamingRecord> records;
			//DbDataReader dbDataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (IDisposableDispatch<DbDataReader> dbDataReader = ResourceManager.__.Using(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE)))
			{
				records = this.GetRecordsFromReader(dbDataReader.Target, recordsAffectedCallback);

				ResourceManager.__.Print(_, "before yield loop");

				foreach (IAdoNetStreamingRecord record in records)
				{
					ResourceManager.__.Print(_, "on yield item");

					yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				ResourceManager.__.Print(_, "after yield loop");
			}

			ResourceManager.__.Leave(_);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of results, each with an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of dictionaries with record key/value pairs of data. </returns>
		public IEnumerable<IAdoNetStreamingResult> ExecuteResults(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			IEnumerable<IAdoNetStreamingResult> results;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (IDisposableDispatch<DbDataReader> dbDataReader = ResourceManager.__.Using(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE)))
			{
				results = this.GetResultsFromReader(dbDataReader.Target);

				ResourceManager.__.Print(_, "before yield loop");

				foreach (IAdoNetStreamingResult result in results)
				{
					ResourceManager.__.Print(_, "on yield item");

					yield return result; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				ResourceManager.__.Print(_, "after yield loop");
			}

			ResourceManager.__.Leave(_);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IAdoNetStreamingRecord> ExecuteSchemaRecords(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback)
		{
			IEnumerable<IAdoNetStreamingRecord> records;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (IDisposableDispatch<DbDataReader> dbDataReader = ResourceManager.__.Using(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE)))
			{
				records = this.GetSchemaRecordsFromReader(dbDataReader.Target, recordsAffectedCallback);

				ResourceManager.__.Print(_, "before yield loop");

				foreach (IAdoNetStreamingRecord record in records)
				{
					ResourceManager.__.Print(_, "on yield item");

					yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				ResourceManager.__.Print(_, "after yield loop");
			}

			ResourceManager.__.Leave(_);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an results, each with an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// This method DOES NOT DISPOSE OF CONNECTION/TRANSACTION - UP TO THE CALLER.
		/// </summary>
		/// <param name="dbConnection"> The database connection. </param>
		/// <param name="dbTransaction"> An optional local database transaction. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IAdoNetStreamingResult> ExecuteSchemaResults(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			IEnumerable<IAdoNetStreamingResult> results;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.SchemaOnly;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			using (IDisposableDispatch<DbDataReader> dbDataReader = ResourceManager.__.Using(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE)))
			{
				results = this.GetSchemaResultsFromReader(dbDataReader.Target);

				ResourceManager.__.Print(_, "before yield loop");

				foreach (IAdoNetStreamingResult result in results)
				{
					ResourceManager.__.Print(_, "on yield item");

					yield return result; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				ResourceManager.__.Print(_, "after yield loop");
			}

			ResourceManager.__.Leave(_);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of record dictionary instances, containing key/value pairs of data. </returns>
		public IEnumerable<IAdoNetStreamingRecord> GetRecordsFromReader(DbDataReader dbDataReader, Action<int> recordsAffectedCallback)
		{
			AdoNetStreamingRecord record;
			int recordsAffected;
			long recordIndex = 0;
			string key;
			object value;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			ResourceManager.__.Watching(_, dbDataReader);
			{
				ResourceManager.__.Print(_, "before yield loop");

				while (dbDataReader.Read())
				{
					record = new AdoNetStreamingRecord(-1, recordIndex);

					for (int fieldIndex = 0; fieldIndex < dbDataReader.FieldCount; fieldIndex++)
					{
						key = dbDataReader.GetName(fieldIndex);
						value = dbDataReader.GetValue(fieldIndex);
						value = value.ChangeType<object>();

						if (record.ContainsKey(key) || (key ?? string.Empty).Length == 0)
							key = string.Format("Field_{0:0000}", fieldIndex);

						record.Add(key, value);
					}

					ResourceManager.__.Print(_, "on yield item");

					recordIndex++;
					yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				ResourceManager.__.Print(_, "after yield loop");
			}

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			ResourceManager.__.Leave(_);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader to an enumerable of results, each with an enumerable of records.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of dictionaries with record key/value pairs of data. </returns>
		public IEnumerable<IAdoNetStreamingResult> GetResultsFromReader(DbDataReader dbDataReader)
		{
			long resultIndex = 0;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			ResourceManager.__.Watching(_, dbDataReader);
			{
				ResourceManager.__.Print(_, "before yield loop");

				do
				{
					IEnumerable<IAdoNetStreamingRecord> records;
					AdoNetStreamingResult result;

					result = new AdoNetStreamingResult(resultIndex);
					records = this.GetRecordsFromReader(dbDataReader, (ra) =>
																	{
																		AdoNetStreamingResult _result = result; // prevent modified closure
																		_result.RecordsAffected = ra;
																	});
					result.Records = records;

					ResourceManager.__.Print(_, "on yield item");

					resultIndex++;
					yield return result; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (dbDataReader.NextResult());

				ResourceManager.__.Print(_, "after yield loop");
			}

			ResourceManager.__.Leave(_);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of record dictionaries.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <param name="recordsAffectedCallback"> Executed when the output count of records affected is available to return (post enumeration). </param>
		/// <returns> An enumerable of record dictionary instances, containing key/value pairs of schema metadata. </returns>
		public IEnumerable<IAdoNetStreamingRecord> GetSchemaRecordsFromReader(DbDataReader dbDataReader, Action<int> recordsAffectedCallback)
		{
			ReadOnlyCollection<DbColumn> dbColumns;
			DbColumn dbColumn;
			PropertyInfo[] propertyInfos;
			PropertyInfo propertyInfo;
			AdoNetStreamingRecord record;
			int recordsAffected;
			string key;
			object value;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			if (!dbDataReader.CanGetColumnSchema())
				throw new NotSupportedException(string.Format("The connection command type '{0}' does not support schema access.", dbDataReader.GetType().FullName));

			ResourceManager.__.Watching(_, dbDataReader);
			{
				dbColumns = dbDataReader.GetColumnSchema();

				ResourceManager.__.Print(_, "before yield loop");

				if ((object)dbColumns != null)
				{
					for (long recordIndex = 0; recordIndex < dbColumns.Count; recordIndex++)
					{
						dbColumn = dbColumns[(int)recordIndex];

						propertyInfos = dbColumn.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

						record = new AdoNetStreamingRecord(-1, recordIndex);

						if ((object)propertyInfos != null)
						{
							for (int fieldIndex = 0; fieldIndex < propertyInfos.Length; fieldIndex++)
							{
								propertyInfo = propertyInfos[fieldIndex];

								if (propertyInfo.GetIndexParameters().Any())
									continue;

								key = propertyInfo.Name;
								value = propertyInfo.GetValue(dbColumn);
								value = value.ChangeType<object>();

								record.Add(key, value);
							}
						}

						ResourceManager.__.Print(_, "on yield item");

						yield return record; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
					}
				}

				ResourceManager.__.Print(_, "after yield loop");
			}

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			ResourceManager.__.Leave(_);
		}

		/// <summary>
		/// Execute a command against a data source, mapping the data reader GetSchemaTable() result to an enumerable of results, each with an enumerable of records.
		/// This method performs LAZY LOADING/DEFERRED EXECUTION.
		/// Note that THE DATA READER WILL NOT BE DISPOSED UPON ENUMERATION OR FOREACH BRANCH OUT.
		/// </summary>
		/// <param name="dbDataReader"> The target data reader. </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of dictionaries with record key/value pairs of schema metadata. </returns>
		public IEnumerable<IAdoNetStreamingResult> GetSchemaResultsFromReader(DbDataReader dbDataReader)
		{
			long resultIndex = 0;

			Guid? _ = ResourceManager.__.Enter();

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			ResourceManager.__.Watching(_, dbDataReader);
			{
				ResourceManager.__.Print(_, "before yield loop");

				do
				{
					IEnumerable<IAdoNetStreamingRecord> records;
					AdoNetStreamingResult result;

					result = new AdoNetStreamingResult(resultIndex);
					records = this.GetSchemaRecordsFromReader(dbDataReader, (ra) =>
																			{
																				AdoNetStreamingResult _result = result; // prevent modified closure
																				_result.RecordsAffected = ra;
																			});
					result.Records = records;

					ResourceManager.__.Print(_, "on yield item");

					resultIndex++;
					yield return result; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (dbDataReader.NextResult());

				ResourceManager.__.Print(_, "after yield loop");
			}

			ResourceManager.__.Leave(_);
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		/// <summary>
		/// http://www.yoda.arachsys.com/csharp/singleton.html
		/// </summary>
		private class ResourceManager
		{
			#region Constructors/Destructors

			/// <summary>
			/// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			/// </summary>
			static ResourceManager()
			{
			}

			#endregion

			#region Fields/Constants

			internal static readonly IResourceManager __ = __;

			#endregion
		}

		#endregion
	}
}