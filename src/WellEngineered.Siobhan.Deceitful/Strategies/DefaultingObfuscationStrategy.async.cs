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
	/// Returns an alternate value that is always null if NULL or the default value if NOT NULL.
	/// DATA TYPE: any
	/// </summary>
	public sealed partial class DefaultingObfuscationStrategy : ObfuscationStrategy<DefaultingObfuscationStrategy.Spec>
	{
		#region Methods/Operators

		protected override async ValueTask<object> CoreGetObfuscatedValueAsync(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken)
		{
			object value;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			value = GetDefault(field.IsFieldOptional && (this.Specification.DefaultingCanBeNull ?? false), field.FieldType);

			await Task.CompletedTask;
			return value;
		}

		#endregion
	}
}
#endif