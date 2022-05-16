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

namespace WellEngineered.Siobhan.Relational
{
	public partial class AdoNetStreamingPayloadDataReader : DbDataReader
	{
		#region Constructors/Destructors

		private AdoNetStreamingPayloadDataReader(ISiobhanSchema schema, IEnumerable<IAdoNetStreamingResult> results)
		{
			IEnumerator<IAdoNetStreamingResult> resultz;

			if ((object)schema == null)
				throw new ArgumentNullException(nameof(schema));

			if ((object)results == null)
				throw new ArgumentNullException(nameof(results));

			resultz = results.GetEnumerator();

			if ((object)resultz == null)
				throw new InvalidOperationException(nameof(resultz));

			this.schema = schema;
			this.results = results;
			this.resultz = resultz;

			this.fieldCache = schema.Fields.Select((f, i) => new
															{
																FieldName = f.Key,
																FieldOrdinal = i
															}).ToDictionary(p => p.FieldName, p => p.FieldOrdinal, StringComparer.CurrentCultureIgnoreCase);
			// is this needed?
			if (!this.AdvanceResult())
				throw new InvalidOperationException(nameof(this.AdvanceResult));
		}

		#endregion

		#region Fields/Constants

		private readonly IDictionary<string, int> fieldCache;

		private readonly ISiobhanSchema schema;
		private string[] currentKeys;
		private object[] currentValues;
		private bool? isRecordsEnumerableClosed;
		private bool? isResultsEnumerableClosed;
		private IEnumerable<IAdoNetStreamingRecord> records;
		private IEnumerator<IAdoNetStreamingRecord> recordz;
		private IEnumerable<IAdoNetStreamingResult> results;
		private IEnumerator<IAdoNetStreamingResult> resultz;

		#endregion

		#region Properties/Indexers/Events

		public override object this[string name]
		{
			get
			{
				return this.HasRecord ? this.Recordz.Current[name] : null;
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.HasRecord ? this.CurrentValues[i] : null;
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
				return this.HasRecord ? this.Recordz.Current.Keys.Count() : 0;
			}
		}

		private bool HasRecord
		{
			get
			{
				return this.HasResult && (object)this.Recordz.Current != null;
			}
		}

		private bool HasResult
		{
			get
			{
				return (object)this.Resultz.Current != null;
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
				return this.IsRecordsEnumerableClosed ?? false;
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return -1;
			}
		}

		private IEnumerable<IAdoNetStreamingResult> Results
		{
			get
			{
				return this.results;
			}
		}

		private IEnumerator<IAdoNetStreamingResult> Resultz
		{
			get
			{
				return this.resultz;
			}
		}

		private ISiobhanSchema Schema
		{
			get
			{
				return this.schema;
			}
		}

		public override int VisibleFieldCount
		{
			get
			{
				return 0;
			}
		}

		private string[] CurrentKeys
		{
			get
			{
				return this.currentKeys;
			}
			set
			{
				this.currentKeys = value;
			}
		}

		private object[] CurrentValues
		{
			get
			{
				return this.currentValues;
			}
			set
			{
				this.currentValues = value;
			}
		}

		protected bool? IsRecordsEnumerableClosed
		{
			get
			{
				return this.isRecordsEnumerableClosed;
			}
			set
			{
				this.isRecordsEnumerableClosed = value;
			}
		}

		protected bool? IsResultsEnumerableClosed
		{
			get
			{
				return this.isResultsEnumerableClosed;
			}
			set
			{
				this.isResultsEnumerableClosed = value;
			}
		}

		private IEnumerable<IAdoNetStreamingRecord> Records
		{
			get
			{
				return this.records;
			}
			set
			{
				this.records = value;
			}
		}

		private IEnumerator<IAdoNetStreamingRecord> Recordz
		{
			get
			{
				return this.recordz;
			}
			set
			{
				this.recordz = value;
			}
		}

		#endregion

		#region Methods/Operators

		private bool AdvanceResult()
		{
			if (!(this.IsResultsEnumerableClosed ?? false))
			{
				bool retval = !(bool)(this.IsResultsEnumerableClosed = !this.Resultz.MoveNext());

				if (retval && this.HasResult)
				{
					IAdoNetStreamingResult result;

					result = this.Resultz.Current;

					if ((object)result == null)
						throw new ArgumentNullException(nameof(result));

					this.Records = result.Records;

					if ((object)this.Records == null)
						throw new ArgumentNullException(nameof(this.Records));

					this.Recordz = this.Records.GetEnumerator();

					if ((object)this.Recordz == null)
						throw new InvalidOperationException(nameof(this.Recordz));
				}

				return retval;
			}
			else
				return true;
		}

		public override void Close()
		{
			IDisposable disposable;

			disposable = this.Recordz;

			if ((object)disposable != null)
				disposable.Dispose();

			disposable = this.Records as IDisposable;

			if ((object)disposable != null)
				disposable.Dispose();

			disposable = this.Resultz;

			if ((object)disposable != null)
				disposable.Dispose();

			disposable = this.Results as IDisposable;

			if ((object)disposable != null)
				disposable.Dispose();
		}

		public override Task CloseAsync()
		{
			return base.CloseAsync();
		}

		public override ValueTask DisposeAsync()
		{
			return base.DisposeAsync();
		}

		public override bool GetBoolean(int i)
		{
			return this.HasRecord ? (Boolean)this.CurrentValues[i] : default(Boolean);
		}

		public override byte GetByte(int i)
		{
			return this.HasRecord ? (Byte)this.CurrentValues[i] : default(Byte);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return 0;
		}

		public override char GetChar(int i)
		{
			return this.HasRecord ? (Char)this.CurrentValues[i] : default(Char);
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
			return this.HasRecord ? (DateTime)this.CurrentValues[i] : default(DateTime);
		}

		public override decimal GetDecimal(int i)
		{
			return this.HasRecord ? (Decimal)this.CurrentValues[i] : default(Decimal);
		}

		public override double GetDouble(int i)
		{
			return this.HasRecord ? (Double)this.CurrentValues[i] : default(Double);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.Resultz;
		}

		public override Type GetFieldType(int i)
		{
			return this.HasRecord && (object)this.CurrentValues[i] != null ? this.CurrentValues[i].GetType() : null;
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
			return this.HasRecord ? (Single)this.CurrentValues[i] : default(Single);
		}

		public override Guid GetGuid(int i)
		{
			return this.HasRecord ? (Guid)this.CurrentValues[i] : default(Guid);
		}

		public override short GetInt16(int i)
		{
			return this.HasRecord ? (Int16)this.CurrentValues[i] : default(Int16);
		}

		public override int GetInt32(int i)
		{
			return this.HasRecord ? (Int32)this.CurrentValues[i] : default(Int32);
		}

		public override long GetInt64(int i)
		{
			return this.HasRecord ? (Int64)this.CurrentValues[i] : default(Int64);
		}

		public override string GetName(int i)
		{
			return this.HasRecord ? this.CurrentKeys[i] : null;
		}

		public override int GetOrdinal(string name)
		{
			if (this.Schema.Fields.TryGetValue(name, out ISiobhanField field))
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
			return this.HasRecord ? (String)this.CurrentValues[i] : default(String);
		}

		public override TextReader GetTextReader(int ordinal)
		{
			return base.GetTextReader(ordinal);
		}

		public override object GetValue(int i)
		{
			return this.HasRecord ? (Object)this.CurrentValues[i] : default(Object);
		}

		public override int GetValues(object[] values)
		{
			return 0;
		}

		public override bool IsDBNull(int i)
		{
			return this.HasRecord ? (object)this.CurrentValues[i] == null : true;
		}

		public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
		{
			return base.IsDBNullAsync(ordinal, cancellationToken);
		}

		public override bool NextResult()
		{
			return this.AdvanceResult();
		}

		public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			return base.NextResultAsync(cancellationToken);
		}

		public override bool Read()
		{
			if (!(this.IsRecordsEnumerableClosed ?? false))
			{
				bool retval = !(bool)(this.IsRecordsEnumerableClosed = !this.Recordz.MoveNext());

				if (retval && this.HasRecord)
				{
					this.CurrentKeys = this.Recordz.Current.Keys.ToArray();
					this.CurrentValues = this.Recordz.Current.Values.ToArray();
				}

				return retval;
			}
			else
				return true;
		}

		public override Task<bool> ReadAsync(CancellationToken cancellationToken)
		{
			return base.ReadAsync(cancellationToken);
		}

		#endregion

		/*

		public override void Close()
		{
			IDisposable disposable;

			disposable = this.Recordz;

			if ((object)disposable != null)
				disposable.Dispose();

			disposable = this.Records as IDisposable;

			if ((object)disposable != null)
				disposable.Dispose();

			disposable = this.Resultz;

			if ((object)disposable != null)
				disposable.Dispose();

			disposable = this.Results as IDisposable;

			if ((object)disposable != null)
				disposable.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			return base.GetDbDataReader(ordinal);
		}

		public override bool GetBoolean(int i)
		{
			return (bool)this.Recordz.Current;
		}

		public override byte GetByte(int i)
		{
			return base.GetByte(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length)
		{
			return base.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
		}

		public override char GetChar(int i)
		{
			return base.GetChar(i);
		}

		public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length)
		{
			return base.GetChars(i, fieldOffset, buffer, bufferOffset, length);
		}

		public override string GetDataTypeName(int i)
		{
			return base.GetDataTypeName(i);
		}

		public override DateTime GetDateTime(int i)
		{
			return this.HasRecord ? (DateTime)this.CurrentValues[i] : default(DateTime);
		}

		public override decimal GetDecimal(int i)
		{
			return this.HasRecord ? (Decimal)this.CurrentValues[i] : default(Decimal);
		}

		public override double GetDouble(int i)
		{
			return this.HasRecord ? (Double)this.CurrentValues[i] : default(Double);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.Recordz;
		}

		public override Type GetFieldType(int i)
		{
			return this.HasRecord && (object)this.CurrentValues[i] != null ? this.CurrentValues[i].GetType() : null;
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
			return this.HasRecord ? (Single)this.CurrentValues[i] : default(Single);
		}

		public override Guid GetGuid(int i)
		{
			return this.HasRecord ? (Guid)this.CurrentValues[i] : default(Guid);
		}

		public override short GetInt16(int i)
		{
			return this.HasRecord ? (Int16)this.CurrentValues[i] : default(Int16);
		}

		public override int GetInt32(int i)
		{
			return this.HasRecord ? (Int32)this.CurrentValues[i] : default(Int32);
		}

		public override long GetInt64(int i)
		{
			return this.HasRecord ? (Int64)this.CurrentValues[i] : default(Int64);
		}

		public override string GetName(int i)
		{
			return this.HasRecord ? this.CurrentKeys[i] : null;
		}

		public override int GetOrdinal(string name)
		{
			if (this.Schema.Fields.TryGetValue(name, out ISiobhanField field))
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
			return this.HasRecord ? (String)this.CurrentValues[i] : default(String);
		}

		public override TextReader GetTextReader(int ordinal)
		{
			return base.GetTextReader(ordinal);
		}

		public override object GetValue(int i)
		{
			return this.HasRecord ? (Object)this.CurrentValues[i] : default(Object);
		}

		public override int GetValues(object[] values)
		{
			return 0;
		}

		public override bool IsDBNull(int i)
		{
			return this.HasRecord ? (object)this.CurrentValues[i] == null : true;
		}

		public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
		{
			return base.IsDBNullAsync(ordinal, cancellationToken);
		}

		public override bool NextResult()
		{
			if (!(this.IsResultsEnumerableClosed ?? false))
			{
				bool retval = !(bool)(this.IsResultsEnumerableClosed = !this.Resultz.MoveNext());

				if (retval && this.HasResult)
				{
					IAdoNetStreamingResult result;

					result = this.Resultz.Current;

					if ((object)result == null)
						throw new ArgumentNullException(nameof(result));

					this.Records = result.Records;

					if ((object)this.Records == null)
						throw new ArgumentNullException(nameof(this.Records));

					this.Recordz = this.Records.GetEnumerator();

					if ((object)this.Recordz == null)
						throw new InvalidOperationException(nameof(this.Recordz));
				}

				return retval;
			}
			else
				return true;
		}

		public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			return base.NextResultAsync(cancellationToken);
		}

		public override bool Read()
		{
			if (!(this.IsRecordsEnumerableClosed ?? false))
			{
				bool retval = !(bool)(this.IsRecordsEnumerableClosed = !this.TargetPayloadEnumerator.MoveNext());

				if (retval && this.HasRecord)
				{
					this.CurrentKeys = this.TargetPayloadEnumerator.Current.Keys.ToArray();
					this.CurrentValues = this.TargetPayloadEnumerator.Current.Values.ToArray();
				}

				return retval;
			}
			else
				return true;
		}

		public override Task<bool> ReadAsync(CancellationToken cancellationToken)
		{
			return base.ReadAsync(cancellationToken);
		}
*/
	}
}