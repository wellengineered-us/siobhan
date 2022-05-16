/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns an alternate value using a hashed lookup into a dictionary.
	/// DATA TYPE: string
	/// </summary>
	public sealed partial class SubstitutionObfuscationStrategy : ObfuscationStrategy<SubstitutionObfuscationStrategy.Spec>
	{
		#region Methods/Operators

		protected override async ValueTask<object> CoreGetObfuscatedValueAsync(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken)
		{
			Type valueType;
			string _columnValue;

			long surrogateKey;
			object surrogateValue;

			DictionaryConfiguration dictionaryConfiguration;

			Tuple<bool, object> result;

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

			surrogateKey = await obfuscationContext.GetValueHashAsync(dictionaryConfiguration.RecordCount, originalFieldValue, cancellationToken);

			result = await obfuscationContext.TryGetSurrogateValueAsync(dictionaryConfiguration, surrogateKey, cancellationToken);

			if ((object)result == null || !result.Item1)
				throw new InvalidOperationException(string.Format("Failed to obtain surrogate value."));

			return surrogateValue = result.Item2;
		}

		#endregion
	}
}
#endif