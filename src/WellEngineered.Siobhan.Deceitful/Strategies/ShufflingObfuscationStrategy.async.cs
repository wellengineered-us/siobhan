/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns an alternate value using a hashed shuffle of alphanumeric characters (while preserving other characters).
	/// DATA TYPE: string
	/// </summary>
	public sealed partial class ShufflingObfuscationStrategy : ObfuscationStrategy<ShufflingObfuscationStrategy.Spec>
	{
		#region Methods/Operators

		protected override async ValueTask<object> CoreGetObfuscatedValueAsync(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken)
		{
			long valueHash;
			object value;
			long randomSeed;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			valueHash = await obfuscationContext.GetValueHashAsync(null, originalFieldValue, cancellationToken);
			randomSeed = valueHash;

			value = await this.GetShuffleAsync(randomSeed, originalFieldValue, cancellationToken);

			return value;
		}

		private async Task<object> GetShuffleAsync(long randomSeed, object value, CancellationToken cancellationToken)
		{
			Type valueType;
			string _value;

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (string.IsNullOrWhiteSpace(_value))
				return _value;

			_value = _value.Trim();

			Dictionary<int, char> fidelityMap = ImplNormalize(ref _value);

			_value = FisherYatesShuffle((int)randomSeed, _value);

			ImplDenormalize(fidelityMap, ref _value);

			await Task.CompletedTask;
			return _value;
		}

		#endregion
	}
}
#endif