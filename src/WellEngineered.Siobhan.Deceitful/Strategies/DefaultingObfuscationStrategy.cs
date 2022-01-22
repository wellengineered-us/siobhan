/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns an alternate value that is always null if NULL or the default value if NOT NULL.
	/// DATA TYPE: any
	/// </summary>
	public sealed partial class DefaultingObfuscationStrategy : ObfuscationStrategy<DefaultingObfuscationStrategy.Spec>
	{
		#region Constructors/Destructors

		public DefaultingObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static object GetDefault(bool isNullable, Type valueType)
		{
			if ((object)valueType == null)
				throw new ArgumentNullException(nameof(valueType));

			if (valueType == typeof(String))
				return isNullable ? null : string.Empty;

			if (isNullable)
				valueType = valueType.MakeNullableType();

			return valueType.DefaultValue();
		}

		protected override object CoreGetObfuscatedValue(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue)
		{
			object value;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			value = GetDefault(field.IsFieldOptional && (this.Specification.DefaultingCanBeNull ?? false), field.FieldType);

			return value;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public sealed class Spec : SiobhanSpecification
		{
			#region Constructors/Destructors

			public Spec()
			{
			}

			#endregion

			#region Fields/Constants

			private bool? defaultingCanBeNull;

			#endregion

			#region Properties/Indexers/Events

			public bool? DefaultingCanBeNull
			{
				get
				{
					return this.defaultingCanBeNull;
				}
				set
				{
					this.defaultingCanBeNull = value;
				}
			}

			#endregion
		}

		#endregion
	}
}