/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace WellEngineered.Siobhan.Model
{
	public sealed class SiobhanSiobhanSchemaBuilder : ISiobhanSchemaBuilder, ISiobhanSchema
	{
		#region Constructors/Destructors

		public SiobhanSiobhanSchemaBuilder()
			: this(new Dictionary<string, ISiobhanField>(StringComparer.OrdinalIgnoreCase))
		{
		}

		public SiobhanSiobhanSchemaBuilder(Dictionary<string, ISiobhanField> fields)
		{
			if ((object)fields == null)
				throw new ArgumentNullException(nameof(fields));

			this.fields = fields;
		}

		#endregion

		#region Fields/Constants

		private static readonly ISiobhanSchema empty = Create().WithVersion(0).Build();
		private readonly Dictionary<string, ISiobhanField> fields;
		private string schemaName;
		private SiobhanSchemaType schemaType;
		private int schemaVersion;

		#endregion

		#region Properties/Indexers/Events

		public static ISiobhanSchema Empty
		{
			get
			{
				return empty;
			}
		}

		public IReadOnlyDictionary<string, ISiobhanField> Fields
		{
			get
			{
				return this.fields;
			}
		}

		private IDictionary<string, ISiobhanField> MutableFields
		{
			get
			{
				return this.fields;
			}
		}

		public ISiobhanSchema Schema
		{
			get
			{
				return this.Build();
			}
		}

		public string SchemaName
		{
			get
			{
				return this.schemaName;
			}
			private set
			{
				this.schemaName = value;
			}
		}

		public SiobhanSchemaType SchemaType
		{
			get
			{
				return this.schemaType;
			}
			private set
			{
				this.schemaType = value;
			}
		}

		public int SchemaVersion
		{
			get
			{
				return this.schemaVersion;
			}
			private set
			{
				this.schemaVersion = value;
			}
		}

		#endregion

		#region Methods/Operators

		private static void AssertCanSet(string propertyName, object propertyValue, object newValue)
		{
			if (propertyValue != null && propertyValue != newValue)
				throw new InvalidOperationException(string.Format("SiobhanSiobhanSchemaBuilder: Property '{0}' has already been set.", propertyName));
		}

		private static void AssertCanSet(string propertyName, bool propertyValue, bool newValue)
		{
			if (propertyValue != false && propertyValue != newValue)
				throw new InvalidOperationException(string.Format("SiobhanSiobhanSchemaBuilder: Property '{0}' has already been set.", propertyName));
		}

		private static void AssertCanSet(string propertyName, int propertyValue, int newValue)
		{
			if (propertyValue != 0 && propertyValue != newValue)
				throw new InvalidOperationException(string.Format("SiobhanSiobhanSchemaBuilder: Property '{0}' has already been set.", propertyName));
		}

		public static SiobhanSiobhanSchemaBuilder Create()
		{
			return new SiobhanSiobhanSchemaBuilder() { SchemaType = SiobhanSchemaType.Object };
		}

		public static ISiobhanSchema FromType(Type type)
		{
			SiobhanSiobhanSchemaBuilder siobhanSchemaBuilder;

			if ((object)type == null)
				throw new ArgumentNullException(nameof(type));

			siobhanSchemaBuilder = new SiobhanSiobhanSchemaBuilder();

			return siobhanSchemaBuilder.Schema;
		}

		public SiobhanSiobhanSchemaBuilder AddField(string fieldName, Type fieldType, bool isFieldOptional, bool isFieldKeyPart, ISiobhanSchema fieldSchema = null)
		{
			if ((object)fieldName == null)
				throw new ArgumentNullException(nameof(fieldName));

			if ((object)fieldType == null)
				throw new ArgumentNullException(nameof(fieldType));

			this.MutableFields.Add(fieldName, new SiobhanField()
											{
												FieldIndex = this.MutableFields.Count,
												FieldName = fieldName,
												FieldType = fieldType,
												IsFieldOptional = isFieldOptional,
												IsFieldKeyComponent = isFieldKeyPart,
												FieldSchema = fieldSchema
											});
			return this;
		}

		public SiobhanSiobhanSchemaBuilder AddFields(IEnumerable<ISiobhanField> fields)
		{
			if ((object)fields == null)
				throw new ArgumentNullException(nameof(fields));

			foreach (ISiobhanField field in fields)
				this.MutableFields.Add(field.FieldName, field);

			return this;
		}

		public ISiobhanSchema Build()
		{
			return this;
		}

		public override string ToString()
		{
			return string.Join(", ", this.Fields.Values.Select(f => $"[{f.FieldName}]@{f.FieldIndex}"));
		}

		public SiobhanSiobhanSchemaBuilder WithName(string value)
		{
			AssertCanSet(nameof(this.SchemaName), this.SchemaName, value);
			this.SchemaName = value;
			return this;
		}

		public SiobhanSiobhanSchemaBuilder WithType(SiobhanSchemaType value)
		{
			AssertCanSet(nameof(this.SchemaType), this.SchemaType, value);
			this.SchemaType = value;
			return this;
		}

		public SiobhanSiobhanSchemaBuilder WithVersion(int value)
		{
			AssertCanSet(nameof(this.SchemaVersion), this.SchemaVersion, value);
			this.SchemaVersion = value;
			return this;
		}

		#endregion
	}
}