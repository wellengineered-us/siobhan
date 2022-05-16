/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace WellEngineered.Siobhan.Model
{
	public class SiobhanKeyValueView : ISiobhanKeyValueView
	{
		#region Constructors/Destructors

		public SiobhanKeyValueView(ISiobhanSchema originalSchema, ISiobhanPayload originalPayload)
		{
			ISiobhanPayload key, value;
			ISiobhanSchemaBuilder k, v;
			IEnumerable<IGrouping<bool, ISiobhanField>> groups;

			if ((object)originalSchema == null)
				throw new ArgumentNullException(nameof(originalSchema));

			if ((object)originalPayload == null)
				throw new ArgumentNullException(nameof(originalPayload));

			groups = originalSchema.Fields.Values.OrderBy(f => f.FieldIndex).GroupBy(f => f.IsFieldKeyComponent);

			key = new SiobhanPayload();
			value = new SiobhanPayload();
			k = new SiobhanSiobhanSchemaBuilder();
			v = new SiobhanSiobhanSchemaBuilder();

			foreach (IGrouping<bool, ISiobhanField> grouping in groups)
			{
				foreach (ISiobhanField field in grouping)
				{
					originalPayload.TryGetValue(field.FieldName, out object fieldValue);

					(grouping.Key ? key : value).Add(field.FieldName, fieldValue);
					(grouping.Key ? k : v).AddField(field.FieldName, field.FieldType, field.IsFieldOptional, field.IsFieldKeyComponent, field.FieldSchema);
				}
			}

			this.originalSchema = originalSchema;
			this.originalPayload = originalPayload;
			this.keyPayload = key;
			this.valuePayload = value;
			this.keySchema = k.Build();
			this.valueSchema = v.Build();
		}

		public SiobhanKeyValueView(ISiobhanSchema originalSchema, ISiobhanPayload originalPayload,
			ISiobhanSchema keySchema, ISiobhanPayload keyPayload,
			ISiobhanSchema valueSchema, ISiobhanPayload valuePayload)
		{
			if ((object)originalSchema == null)
				throw new ArgumentNullException(nameof(originalSchema));

			if ((object)originalPayload == null)
				throw new ArgumentNullException(nameof(originalPayload));

			if ((object)keySchema == null)
				throw new ArgumentNullException(nameof(keySchema));

			if ((object)keyPayload == null)
				throw new ArgumentNullException(nameof(keyPayload));

			if ((object)valueSchema == null)
				throw new ArgumentNullException(nameof(keySchema));

			if ((object)valuePayload == null)
				throw new ArgumentNullException(nameof(valuePayload));

			this.originalSchema = originalSchema;
			this.originalPayload = originalPayload;
			this.keySchema = keySchema;
			this.keyPayload = keyPayload;
			this.valueSchema = valueSchema;
			this.valuePayload = valuePayload;
		}

		#endregion

		#region Fields/Constants

		private readonly ISiobhanPayload keyPayload;
		private readonly ISiobhanSchema keySchema;
		private readonly ISiobhanPayload originalPayload;
		private readonly ISiobhanSchema originalSchema;
		private readonly ISiobhanPayload valuePayload;
		private readonly ISiobhanSchema valueSchema;

		#endregion

		#region Properties/Indexers/Events

		public ISiobhanPayload KeyPayload
		{
			get
			{
				return this.keyPayload;
			}
		}

		public ISiobhanSchema KeySchema
		{
			get
			{
				return this.keySchema;
			}
		}

		public ISiobhanPayload OriginalPayload
		{
			get
			{
				return this.originalPayload;
			}
		}

		public ISiobhanSchema OriginalSchema
		{
			get
			{
				return this.originalSchema;
			}
		}

		public ISiobhanPayload ValuePayload
		{
			get
			{
				return this.valuePayload;
			}
		}

		public ISiobhanSchema ValueSchema
		{
			get
			{
				return this.valueSchema;
			}
		}

		#endregion
	}
}