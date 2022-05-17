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

using WellEngineered.Siobhan.Model;

namespace WellEngineered.Siobhan.Relational.UoW
{
	/// <summary>
	/// Provides extension methods for unit of work instances.
	/// </summary>
	public static partial class UnitOfWorkExtensionMethods
	{
		#region Methods/Operators

		public static IAsyncEnumerable<ISiobhanPayload> ExecuteRecordsAsync(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> rowsAffectedCallback, CancellationToken cancellationToken)
		{
			IAsyncEnumerable<ISiobhanPayload> asyncRecords;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			asyncRecords = AdoNetStreamingFacade.ExecuteRecordsAsync(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, rowsAffectedCallback, cancellationToken);

			return asyncRecords;
		}

		public static IAsyncEnumerable<IAsyncAdoNetStreamingResult> ExecuteResultsAsync(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, CancellationToken cancellationToken)
		{
			IAsyncEnumerable<IAsyncAdoNetStreamingResult> asyncResults;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			asyncResults = AdoNetStreamingFacade.ExecuteResultsAsync(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, cancellationToken);

			return asyncResults;
		}

		public static IAsyncEnumerable<ISiobhanPayload> ExecuteSchemaRecordsAsync(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> rowsAffectedCallback, CancellationToken cancellationToken)
		{
			IAsyncEnumerable<ISiobhanPayload> asyncRecords;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			asyncRecords = AdoNetStreamingFacade.ExecuteSchemaRecordsAsync(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, rowsAffectedCallback, cancellationToken);

			return asyncRecords;
		}

		public static IAsyncEnumerable<IAsyncAdoNetStreamingResult> ExecuteSchemaResultsAsync(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, CancellationToken cancellationToken)
		{
			IAsyncEnumerable<IAsyncAdoNetStreamingResult> asyncResults;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			asyncResults = AdoNetStreamingFacade.ExecuteSchemaResultsAsync(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, cancellationToken);

			return asyncResults;
		}

		#endregion
	}
}
#endif