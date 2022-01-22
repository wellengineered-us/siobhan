/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives.Configuration;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns an alternate value using a hashed lookup into a dictionary.
	/// DATA TYPE: string
	/// </summary>
	public sealed partial class SubstitutionObfuscationStrategy : ObfuscationStrategy<SubstitutionObfuscationStrategy.Spec>
	{
		#region Constructors/Destructors

		public SubstitutionObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected override object CoreGetObfuscatedValue(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue)
		{
			Type valueType;
			string _columnValue;

			long surrogateKey;
			object surrogateValue;

			DictionaryConfiguration dictionaryConfiguration;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			if ((object)originalFieldValue == null)
				return null;

			valueType = originalFieldValue.GetType();

			if (valueType != typeof(string))
				return null;

			_columnValue = (string)originalFieldValue;
			_columnValue = _columnValue.Trim();

			if (string.IsNullOrWhiteSpace(_columnValue))
				return _columnValue;

			dictionaryConfiguration = this.Specification.DictionaryConfiguration;

			if ((object)dictionaryConfiguration == null)
				throw new InvalidOperationException(string.Format("Failed to obtain dictionary."));

			if ((dictionaryConfiguration.RecordCount ?? 0L) <= 0L)
				return null;

			surrogateKey = obfuscationContext.GetValueHash(dictionaryConfiguration.RecordCount, originalFieldValue);

			if (!obfuscationContext.TryGetSurrogateValue(dictionaryConfiguration, surrogateKey, out surrogateValue))
				throw new InvalidOperationException(string.Format("Failed to obtain surrogate value."));

			return surrogateValue;
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

			private DictionaryConfiguration dictionaryConfiguration;

			#endregion

			#region Properties/Indexers/Events

			public DictionaryConfiguration DictionaryConfiguration
			{
				get
				{
					return this.dictionaryConfiguration;
				}
				set
				{
					this.EnsureParentOnPropertySet(this.dictionaryConfiguration, value);
					this.dictionaryConfiguration = value;
				}
			}

			#endregion
		}

		#endregion
	}
}