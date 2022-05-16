/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Model
{
	public class SiobhanField : ISiobhanField
	{
		#region Constructors/Destructors

		public SiobhanField()
		{
		}

		#endregion

		#region Fields/Constants

		private long fieldIndex;
		private string fieldName;
		private ISiobhanSchema fieldSchema;
		private Type fieldType;
		private bool isFieldKeyComponent;
		private bool isFieldOptional;

		#endregion

		#region Properties/Indexers/Events

		public long FieldIndex
		{
			get
			{
				return this.fieldIndex;
			}
			set
			{
				this.fieldIndex = value;
			}
		}

		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
			set
			{
				this.fieldName = value;
			}
		}

		public ISiobhanSchema FieldSchema
		{
			get
			{
				return this.fieldSchema;
			}
			set
			{
				this.fieldSchema = value;
			}
		}

		public Type FieldType
		{
			get
			{
				return this.fieldType;
			}
			set
			{
				this.fieldType = value;
			}
		}

		public bool IsFieldKeyComponent
		{
			get
			{
				return this.isFieldKeyComponent;
			}
			set
			{
				this.isFieldKeyComponent = value;
			}
		}

		public bool IsFieldOptional
		{
			get
			{
				return this.isFieldOptional;
			}
			set
			{
				this.isFieldOptional = value;
			}
		}

		#endregion
	}
}