/*
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Solder.Injection;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational
{
	public partial class AdoNetStreamingFacade
		: IAdoNetStreamingFacade
	{
		#region Methods/Operators

		public async IAsyncEnumerable<IAdoNetStreamingRecord> ExecuteRecordsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			IAsyncEnumerable<IAdoNetStreamingRecord> asyncRecords;
			//DbDataReader dbDataReader;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			await using (IAsyncDisposableDispatch<DbDataReader> dbDataReader = await ResourceManager.__.UsingAsync(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE), cancellationToken))
			{
				asyncRecords = this.GetRecordsFromReaderAsync(dbDataReader.AsyncTarget, recordsAffectedCallback, cancellationToken);

				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				await foreach (IAdoNetStreamingRecord asyncRecord in asyncRecords.WithCancellation(cancellationToken))
				{
					await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

					yield return asyncRecord; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		public async IAsyncEnumerable<IAsyncAdoNetStreamingResult> ExecuteResultsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			IAsyncEnumerable<IAsyncAdoNetStreamingResult> asyncResults;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			await using (IAsyncDisposableDispatch<DbDataReader> dbDataReader = await ResourceManager.__.UsingAsync(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE), cancellationToken))
			{
				asyncResults = this.GetResultsFromReaderAsync(dbDataReader.AsyncTarget, cancellationToken);

				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				await foreach (IAsyncAdoNetStreamingResult asyncResult in asyncResults.WithCancellation(cancellationToken))
				{
					await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

					yield return asyncResult; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		public async IAsyncEnumerable<IAdoNetStreamingRecord> ExecuteSchemaRecordsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> recordsAffectedCallback, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			IAsyncEnumerable<IAdoNetStreamingRecord> asyncRecords;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.Default;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			await using (IAsyncDisposableDispatch<DbDataReader> dbDataReader = await ResourceManager.__.UsingAsync(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE), cancellationToken))
			{
				asyncRecords = this.GetSchemaRecordsFromReaderAsync(dbDataReader.AsyncTarget, recordsAffectedCallback, cancellationToken);

				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				await foreach (IAdoNetStreamingRecord asyncRecord in asyncRecords.WithCancellation(cancellationToken))
				{
					await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

					yield return asyncRecord; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		public async IAsyncEnumerable<IAsyncAdoNetStreamingResult> ExecuteSchemaResultsAsync(DbConnection dbConnection, DbTransaction dbTransaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			IAsyncEnumerable<IAsyncAdoNetStreamingResult> asyncResults;

			// force no preparation
			const bool COMMAND_PREPARE = false;

			// force provider default timeout
			const object COMMAND_TIMEOUT = null; /*int?*/

			// force command behavior to default; the unit of work will manage connection lifetime
			const CommandBehavior COMMAND_BEHAVIOR = CommandBehavior.SchemaOnly;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbConnection == null)
				throw new ArgumentNullException(nameof(dbConnection));

			// MUST DISPOSE WITHIN A NEW YIELD STATE MACHINE
			await using (IAsyncDisposableDispatch<DbDataReader> dbDataReader = await ResourceManager.__.UsingAsync(_, this.ExecuteReader(dbConnection, dbTransaction, commandType, commandText, commandParameters, COMMAND_BEHAVIOR, (int?)COMMAND_TIMEOUT, COMMAND_PREPARE), cancellationToken))
			{
				asyncResults = this.GetSchemaResultsFromReaderAsync(dbDataReader.AsyncTarget, cancellationToken);

				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				await foreach (IAsyncAdoNetStreamingResult asyncResult in asyncResults.WithCancellation(cancellationToken))
				{
					await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

					yield return asyncResult; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		public async IAsyncEnumerable<IAdoNetStreamingRecord> GetRecordsFromReaderAsync(DbDataReader dbDataReader, Action<int> recordsAffectedCallback, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			AdoNetStreamingRecord asyncRecord;
			int recordsAffected;
			long recordIndex = 0;
			string key;
			object value;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			await ResourceManager.__.WatchingAsync(_, dbDataReader, cancellationToken);
			{
				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				while (await dbDataReader.ReadAsync(cancellationToken))
				{
					asyncRecord = new AdoNetStreamingRecord(-1, recordIndex);

					for (int fieldIndex = 0; fieldIndex < dbDataReader.FieldCount; fieldIndex++)
					{
						key = dbDataReader.GetName(fieldIndex);
						value = dbDataReader.GetValue(fieldIndex);
						value = value.ChangeType<object>();

						if (asyncRecord.ContainsKey(key) || (key ?? string.Empty).Length == 0)
							key = string.Format("Field_{0:0000}", fieldIndex);

						asyncRecord.Add(key, value);
					}

					await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

					recordIndex++;
					yield return asyncRecord; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		public async IAsyncEnumerable<IAsyncAdoNetStreamingResult> GetResultsFromReaderAsync(DbDataReader dbDataReader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			long resultIndex = 0;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			await ResourceManager.__.WatchingAsync(_, dbDataReader, cancellationToken);
			{
				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				do
				{
					IAsyncEnumerable<IAdoNetStreamingRecord> asyncRecords;
					AsyncAdoNetStreamingResult asyncResult;

					asyncResult = new AsyncAdoNetStreamingResult(resultIndex);
					asyncRecords = this.GetRecordsFromReaderAsync(dbDataReader, (ra) =>
																				{
																					AsyncAdoNetStreamingResult _asyncResult = asyncResult; // prevent modified closure
																					_asyncResult.RecordsAffected = ra;
																				}, cancellationToken);
					asyncResult.AsyncRecords = asyncRecords;

					await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

					resultIndex++;
					yield return asyncResult; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (await dbDataReader.NextResultAsync(cancellationToken));

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		public async IAsyncEnumerable<IAdoNetStreamingRecord> GetSchemaRecordsFromReaderAsync(DbDataReader dbDataReader, Action<int> recordsAffectedCallback, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			ReadOnlyCollection<DbColumn> dbColumns;
			DbColumn dbColumn;
			PropertyInfo[] propertyInfos;
			PropertyInfo propertyInfo;
			AdoNetStreamingRecord asyncRecord;
			int recordsAffected;
			string key;
			object value;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			if (!dbDataReader.CanGetColumnSchema())
				throw new NotSupportedException(string.Format("The connection command type '{0}' does not support schema access.", dbDataReader.GetType().FullName));

			await ResourceManager.__.WatchingAsync(_, dbDataReader, cancellationToken);
			{
				dbColumns = dbDataReader.GetColumnSchema();

				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				if ((object)dbColumns != null)
				{
					for (long recordIndex = 0; recordIndex < dbColumns.Count; recordIndex++)
					{
						dbColumn = dbColumns[(int)recordIndex];

						propertyInfos = dbColumn.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);

						asyncRecord = new AdoNetStreamingRecord(-1, recordIndex);

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

								asyncRecord.Add(key, value);
							}
						}

						await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

						yield return asyncRecord; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
					}
				}

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			recordsAffected = dbDataReader.RecordsAffected;

			if ((object)recordsAffectedCallback != null)
				recordsAffectedCallback(recordsAffected);

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		public async IAsyncEnumerable<IAsyncAdoNetStreamingResult> GetSchemaResultsFromReaderAsync(DbDataReader dbDataReader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			long resultIndex = 0;

			Guid? _ = await ResourceManager.__.EnterAsync(cancellationToken);

			if ((object)dbDataReader == null)
				throw new ArgumentNullException(nameof(dbDataReader));

			await ResourceManager.__.WatchingAsync(_, dbDataReader, cancellationToken);
			{
				await ResourceManager.__.PrintAsync(_, "before yield loop", cancellationToken);

				do
				{
					IAsyncEnumerable<IAdoNetStreamingRecord> asyncRecords;
					AsyncAdoNetStreamingResult asyncResult;

					asyncResult = new AsyncAdoNetStreamingResult(resultIndex);
					asyncRecords = this.GetSchemaRecordsFromReaderAsync(dbDataReader, (ra) =>
																					{
																						AsyncAdoNetStreamingResult _asyncResult = asyncResult; // prevent modified closure
																						_asyncResult.RecordsAffected = ra;
																					}, cancellationToken);
					asyncResult.AsyncRecords = asyncRecords;

					await ResourceManager.__.PrintAsync(_, "on yield item", cancellationToken);

					resultIndex++;
					yield return asyncResult; // LAZY PROCESSING INTENT HERE / DO NOT FORCE EAGER LOAD
				}
				while (await dbDataReader.NextResultAsync(cancellationToken));

				await ResourceManager.__.PrintAsync(_, "after yield loop", cancellationToken);
			}

			await ResourceManager.__.LeaveAsync(_, cancellationToken: cancellationToken);
		}

		#endregion
	}
}