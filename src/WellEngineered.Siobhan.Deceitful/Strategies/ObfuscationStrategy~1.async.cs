/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Siobhan.Primitives.Component;
using WellEngineered.Siobhan.Primitives.Configuration;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	public abstract partial class ObfuscationStrategy<TObfuscationStrategySpec>
		: SiobhanComponent<UnknownSiobhanConfiguration<TObfuscationStrategySpec>, TObfuscationStrategySpec>,
			IObfuscationStrategy<TObfuscationStrategySpec>
		where TObfuscationStrategySpec : class, ISiobhanSpecification, new()
	{
		#region Methods/Operators

		protected abstract ValueTask<object> CoreGetObfuscatedValueAsync(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken);

		public async ValueTask<object> GetObfuscatedValueAsync(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken = default)
		{
			object value;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			if ((object)originalFieldValue == DBNull.Value)
				originalFieldValue = null;

			try
			{
				value = await this.CoreGetObfuscatedValueAsync(obfuscationContext, columnConfiguration, field, originalFieldValue, cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The obfuscation strategy failed (see inner exception)."), ex);
			}

			return value;
		}

		#endregion
	}
}
#endif