/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Model;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational
{
	public class AsyncAdoNetStreamingPayloadDataReader : DbDataReader
	{
		#region Constructors/Destructors

		private AsyncAdoNetStreamingPayloadDataReader(ISiobhanSchema asyncSchema, IAsyncEnumerable<IAsyncAdoNetStreamingResult> asyncResults)
		{
			IAsyncEnumerator<IAsyncAdoNetStreamingResult> asyncResultz;

			if ((object)asyncSchema == null)
				throw new ArgumentNullException(nameof(asyncSchema));

			if ((object)asyncResults == null)
				throw new ArgumentNullException(nameof(asyncResults));

			asyncResultz = asyncResults.GetAsyncEnumerator();

			if ((object)asyncResultz == null)
				throw new InvalidOperationException(nameof(asyncResultz));

			this.asyncSchema = asyncSchema;
			this.asyncResults = asyncResults;
			this.asyncResultz = asyncResultz;

			this.asyncFieldCache = asyncSchema.Fields.Select((f, i) => new
																		{
																			FieldName = f.Key,
																			FieldOrdinal = i
																		}).ToDictionary(p => p.FieldName, p => p.FieldOrdinal, StringComparer.CurrentCultureIgnoreCase);
			// is this needed?
			if (!this.AdvanceResultAsync()
					.GetAwaiter()
					.GetResult())
				throw new InvalidOperationException(nameof(this.AdvanceResultAsync));
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<string, int> asyncFieldCache;
		private readonly ISiobhanSchema asyncSchema;
		private string[] asyncCurrentKeys;
		private object[] asyncCurrentValues;
		private IAsyncEnumerable<IAdoNetStreamingRecord> asyncRecords;
		private IAsyncEnumerator<IAdoNetStreamingRecord> asyncRecordz;
		private IAsyncEnumerable<IAsyncAdoNetStreamingResult> asyncResults;
		private IAsyncEnumerator<IAsyncAdoNetStreamingResult> asyncResultz;
		private bool? isAsyncRecordsEnumerableClosed;
		private bool? isAsyncResultsEnumerableClosed;

		#endregion

		#region Properties/Indexers/Events

		public override object this[string name]
		{
			get
			{
				return this.HasRecord ? this.AsyncRecordz.Current[name] : null;
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.HasRecord ? this.AsyncCurrentValues[i] : null;
			}
		}

		private IAsyncEnumerable<IAsyncAdoNetStreamingResult> AsyncResults
		{
			get
			{
				return this.asyncResults;
			}
		}

		private IAsyncEnumerator<IAsyncAdoNetStreamingResult> AsyncResultz
		{
			get
			{
				return this.asyncResultz;
			}
		}

		private ISiobhanSchema AsyncSchema
		{
			get
			{
				return this.asyncSchema;
			}
		}

		public override int Depth
		{
			get
			{
				return 1;
			}
		}

		public override int FieldCount
		{
			get
			{
				return this.HasRecord ? this.AsyncRecordz.Current.Keys.Count() : 0;
			}
		}

		private bool HasRecord
		{
			get
			{
				return this.HasResult && (object)this.AsyncRecordz.Current != null;
			}
		}

		private bool HasResult
		{
			get
			{
				return (object)this.AsyncResultz.Current != null;
			}
		}

		public override bool HasRows
		{
			get
			{
				return this.HasRecord;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return this.IsAsyncRecordsEnumerableClosed ?? false;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return -1;
			}
		}

		public override int VisibleFieldCount
		{
			get
			{
				return 0;
			}
		}

		private string[] AsyncCurrentKeys
		{
			get
			{
				return this.asyncCurrentKeys;
			}
			set
			{
				this.asyncCurrentKeys = value;
			}
		}

		private object[] AsyncCurrentValues
		{
			get
			{
				return this.asyncCurrentValues;
			}
			set
			{
				this.asyncCurrentValues = value;
			}
		}

		private IAsyncEnumerable<IAdoNetStreamingRecord> AsyncRecords
		{
			get
			{
				return this.asyncRecords;
			}
			set
			{
				this.asyncRecords = value;
			}
		}

		private IAsyncEnumerator<IAdoNetStreamingRecord> AsyncRecordz
		{
			get
			{
				return this.asyncRecordz;
			}
			set
			{
				this.asyncRecordz = value;
			}
		}

		protected bool? IsAsyncRecordsEnumerableClosed
		{
			get
			{
				return this.isAsyncRecordsEnumerableClosed;
			}
			set
			{
				this.isAsyncRecordsEnumerableClosed = value;
			}
		}

		protected bool? IsAsyncResultsEnumerableClosed
		{
			get
			{
				return this.isAsyncResultsEnumerableClosed;
			}
			set
			{
				this.isAsyncResultsEnumerableClosed = value;
			}
		}

		#endregion

		#region Methods/Operators

		private async ValueTask<bool> AdvanceResultAsync(CancellationToken cancellationToken = default)
		{
			if (!(this.IsAsyncResultsEnumerableClosed ?? false))
			{
				bool retval = !(bool)(this.IsAsyncResultsEnumerableClosed = !await this.AsyncResultz.MoveNextAsync());

				if (retval && this.HasResult)
				{
					IAsyncAdoNetStreamingResult result;

					result = this.AsyncResultz.Current;

					if ((object)result == null)
						throw new ArgumentNullException(nameof(result));

					this.AsyncRecords = result.AsyncRecords;

					if ((object)this.AsyncRecords == null)
						throw new ArgumentNullException(nameof(this.AsyncRecords));

					this.AsyncRecordz = this.AsyncRecords.GetAsyncEnumerator(cancellationToken);

					if ((object)this.AsyncRecordz == null)
						throw new InvalidOperationException(nameof(this.AsyncRecordz));
				}

				return retval;
			}
			else
				return true;
		}

		public override void Close()
		{
			this.CloseAsync()
				.GetAwaiter()
				.GetResult();
		}

		public override async Task CloseAsync()
		{
			IAsyncDisposable asyncDisposable;

			await this.AsyncRecordz.SafeDisposeAsync();
			await this.AsyncRecords.SafeDisposeAsync();
			await this.AsyncResultz.SafeDisposeAsync();
			await this.AsyncResults.SafeDisposeAsync();
		}

		public override ValueTask DisposeAsync()
		{
			return base.DisposeAsync();
		}

		public override bool GetBoolean(int i)
		{
			return this.HasRecord ? (Boolean)this.AsyncCurrentValues[i] : default(Boolean);
		}

		public override byte GetByte(int i)
		{
			return this.HasRecord ? (Byte)this.AsyncCurrentValues[i] : default(Byte);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return 0;
		}

		public override char GetChar(int i)
		{
			return this.HasRecord ? (Char)this.AsyncCurrentValues[i] : default(Char);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return 0;
		}

		public override string GetDataTypeName(int i)
		{
			return null;
		}

		public override DateTime GetDateTime(int i)
		{
			return this.HasRecord ? (DateTime)this.AsyncCurrentValues[i] : default(DateTime);
		}

		public override decimal GetDecimal(int i)
		{
			return this.HasRecord ? (Decimal)this.AsyncCurrentValues[i] : default(Decimal);
		}

		public override double GetDouble(int i)
		{
			return this.HasRecord ? (Double)this.AsyncCurrentValues[i] : default(Double);
		}

		public override IEnumerator GetEnumerator()
		{
			// return this.AsyncResultz;
			return null;
		}

		public override Type GetFieldType(int i)
		{
			return this.HasRecord && (object)this.AsyncCurrentValues[i] != null ? this.AsyncCurrentValues[i].GetType() : null;
		}

		public override T GetFieldValue<T>(int ordinal)
		{
			return base.GetFieldValue<T>(ordinal);
		}

		public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
		{
			return base.GetFieldValueAsync<T>(ordinal, cancellationToken);
		}

		public override float GetFloat(int i)
		{
			return this.HasRecord ? (Single)this.AsyncCurrentValues[i] : default(Single);
		}

		public override Guid GetGuid(int i)
		{
			return this.HasRecord ? (Guid)this.AsyncCurrentValues[i] : default(Guid);
		}

		public override short GetInt16(int i)
		{
			return this.HasRecord ? (Int16)this.AsyncCurrentValues[i] : default(Int16);
		}

		public override int GetInt32(int i)
		{
			return this.HasRecord ? (Int32)this.AsyncCurrentValues[i] : default(Int32);
		}

		public override long GetInt64(int i)
		{
			return this.HasRecord ? (Int64)this.AsyncCurrentValues[i] : default(Int64);
		}

		public override string GetName(int i)
		{
			return this.HasRecord ? this.AsyncCurrentKeys[i] : null;
		}

		public override int GetOrdinal(string name)
		{
			if (this.AsyncSchema.Fields.TryGetValue(name, out ISiobhanField field))
				return (int)field.FieldIndex;

			return -1;
		}

		public override Type GetProviderSpecificFieldType(int ordinal)
		{
			return base.GetProviderSpecificFieldType(ordinal);
		}

		public override object GetProviderSpecificValue(int ordinal)
		{
			return base.GetProviderSpecificValue(ordinal);
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			return base.GetProviderSpecificValues(values);
		}

		public override DataTable GetSchemaTable()
		{
			return base.GetSchemaTable();
		}

		public override Stream GetStream(int ordinal)
		{
			return base.GetStream(ordinal);
		}

		public override string GetString(int i)
		{
			return this.HasRecord ? (String)this.AsyncCurrentValues[i] : default(String);
		}

		public override TextReader GetTextReader(int ordinal)
		{
			return base.GetTextReader(ordinal);
		}

		public override object GetValue(int i)
		{
			return this.HasRecord ? (Object)this.AsyncCurrentValues[i] : default(Object);
		}

		public override int GetValues(object[] values)
		{
			return 0;
		}

		public override bool IsDBNull(int i)
		{
			return this.HasRecord ? (object)this.AsyncCurrentValues[i] == null : true;
		}

		public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
		{
			return base.IsDBNullAsync(ordinal, cancellationToken);
		}

		public override bool NextResult()
		{
			return this.NextResultAsync()
				.GetAwaiter()
				.GetResult();
		}

		public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			return await this.AdvanceResultAsync(cancellationToken);
		}

		public override bool Read()
		{
			return this.ReadAsync()
				.GetAwaiter()
				.GetResult();
		}

		public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
		{
			if (!(this.IsAsyncRecordsEnumerableClosed ?? false))
			{
				bool retval = !(bool)(this.IsAsyncRecordsEnumerableClosed = !await this.AsyncRecordz.MoveNextAsync(cancellationToken));

				if (retval && this.HasRecord)
				{
					this.AsyncCurrentKeys = this.AsyncRecordz.Current.Keys.ToArray();
					this.AsyncCurrentValues = this.AsyncRecordz.Current.Values.ToArray();
				}

				return retval;
			}
			else
				return true;
		}

		#endregion
	}
}