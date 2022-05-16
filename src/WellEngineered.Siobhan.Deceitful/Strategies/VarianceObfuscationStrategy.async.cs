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
	/// Returns an alternate value within +/- (x%) of the original value.
	/// DATA TYPE: numeric
	/// Returns an alternate value within +/- (x%:365.25d) of the original value.
	/// DATA TYPE: temporal
	/// </summary>
	public sealed partial class VarianceObfuscationStrategy : ObfuscationStrategy<VarianceObfuscationStrategy.Spec>
	{
		#region Methods/Operators

		protected override async ValueTask<object> CoreGetObfuscatedValueAsync(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken)
		{
			long signHash, valueHash;
			object value;
			double varianceFactor;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			signHash = await obfuscationContext.GetSignHashAsync(originalFieldValue, cancellationToken);
			valueHash = await obfuscationContext.GetValueHashAsync(this.Specification.VariancePercentValue, originalFieldValue, cancellationToken);
			varianceFactor = ((((valueHash <= 0 ? 1 : valueHash)) * ((signHash % 2 == 0 ? 1.0 : -1.0))) / 100.0);

			value = GetVariance(varianceFactor, originalFieldValue);

			return value;
		}

		#endregion
	}
}
#endif