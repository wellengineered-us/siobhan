/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

using WellEngineered.Siobhan.Model;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational.UoW
{
	/// <summary>
	/// Provides extension methods for unit of work instances.
	/// </summary>
	public static partial class UnitOfWorkExtensionMethods
	{
		#region Fields/Constants

		private static readonly Lazy<IAdoNetStreamingFacade> adoNetStreamingFascadeFactory = new Lazy<IAdoNetStreamingFacade>(() => new AdoNetStreamingFacade());

		#endregion

		#region Properties/Indexers/Events

		private static IAdoNetStreamingFacade AdoNetStreamingFacade
		{
			get
			{
				return AdoNetStreamingFascadeFactory.Value;
			}
		}

		private static Lazy<IAdoNetStreamingFacade> AdoNetStreamingFascadeFactory
		{
			get
			{
				return adoNetStreamingFascadeFactory;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// An extension method to create a new data parameter from the data source.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="columnSource"> Specifies the column source. </param>
		/// <param name="parameterDirection"> Specifies the parameter direction. </param>
		/// <param name="dbType"> Specifies the parameter provider-(in)dependent type. </param>
		/// <param name="parameterSize"> Specifies the parameter size. </param>
		/// <param name="parameterPrecision"> Specifies the parameter precision. </param>
		/// <param name="parameterScale"> Specifies the parameter scale. </param>
		/// <param name="parameterNullable"> Specifies the parameter nullable-ness. </param>
		/// <param name="parameterName"> Specifies the parameter name. </param>
		/// <param name="parameterValue"> Specifies the parameter value. </param>
		/// <returns> The data parameter with the specified properties set. </returns>
		public static DbParameter CreateParameter(this IUnitOfWork unitOfWork, string columnSource, ParameterDirection parameterDirection, DbType dbType, int parameterSize, byte parameterPrecision, byte parameterScale, bool parameterNullable, string parameterName, object parameterValue)
		{
			DbParameter dbParameter;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			dbParameter = AdoNetStreamingFacade.CreateParameter(unitOfWork.Connection, unitOfWork.Transaction, columnSource, parameterDirection, dbType, parameterSize, parameterPrecision, parameterScale, parameterNullable, parameterName, parameterValue);

			return dbParameter;
		}

		public static IEnumerable<ISiobhanPayload> ExecuteRecords(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> rowsAffectedCallback)
		{
			IEnumerable<ISiobhanPayload> records;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			records = AdoNetStreamingFacade.ExecuteRecords(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, rowsAffectedCallback);

			return records;
		}

		/// <summary>
		/// An extension method to execute a result query operation against a target unit of work.
		/// DO NOT DISPOSE OF UNIT OF WORK CONTEXT - UP TO THE CALLER.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of records with (key/value pairs of data). </returns>
		public static IEnumerable<IAdoNetStreamingResult> ExecuteResults(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			IEnumerable<IAdoNetStreamingResult> results;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			results = AdoNetStreamingFacade.ExecuteResults(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters);

			return results;
		}

		public static TValue ExecuteScalar<TValue>(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			IEnumerable<ISiobhanPayload> records;
			ISiobhanPayload payload;

			object dbValue;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			records = unitOfWork.ExecuteRecords(commandType, commandText, commandParameters, null);

			if ((object)records == null)
				return default(TValue);

			payload = records.SingleOrDefault();

			if ((object)payload == null)
				return default(TValue);

			if (payload.Count != 1)
				return default(TValue);

			var key = payload.Keys.FirstOrDefault();

			if ((object)key == null)
				return default(TValue);

			dbValue = payload[key];

			return dbValue.ChangeType<TValue>();
		}

		public static IEnumerable<ISiobhanPayload> ExecuteSchemaRecords(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters, Action<int> rowsAffectedCallback)
		{
			IEnumerable<ISiobhanPayload> records;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			// DO NOT DISPOSE OF DATA READER HERE - THE YIELD STATE MACHINE BELOW WILL DO THIS
			records = AdoNetStreamingFacade.ExecuteSchemaRecords(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters, rowsAffectedCallback);

			return records;
		}

		/// <summary>
		/// An extension method to execute a result query operation against a target unit of work.
		/// DO NOT DISPOSE OF UNIT OF WORK CONTEXT - UP TO THE CALLER.
		/// </summary>
		/// <param name="unitOfWork"> The target unit of work. </param>
		/// <param name="commandType"> The type of the command. </param>
		/// <param name="commandText"> The SQL text or stored procedure name. </param>
		/// <param name="commandParameters"> The parameters to use during the operation. </param>
		/// <returns> An enumerable of result instances, each containing an enumerable of records with (key/value pairs of schema metadata). </returns>
		public static IEnumerable<IAdoNetStreamingResult> ExecuteSchemaResults(this IUnitOfWork unitOfWork, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
		{
			IEnumerable<IAdoNetStreamingResult> results;

			if ((object)unitOfWork == null)
				throw new ArgumentNullException(nameof(unitOfWork));

			results = AdoNetStreamingFacade.ExecuteSchemaResults(unitOfWork.Connection, unitOfWork.Transaction, commandType, commandText, commandParameters);

			return results;
		}

		#endregion
	}
}