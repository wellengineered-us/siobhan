/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns un-obfuscated, original value.
	/// DATA TYPE: any
	/// </summary>
	public sealed partial class SurrogateKeyObfuscationStrategy : ObfuscationStrategy<SurrogateKeyObfuscationStrategy.Spec>
	{
		#region Methods/Operators

		private static async Task<object> GetSurrogateKeyAsync(long randomSeed, object value, CancellationToken cancellationToken)
		{
			Random random;
			Op op;
			int val;

			Type valueType;
			Int64 _value;

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (!typeof(Int64).IsAssignableFrom(valueType))
				return null;

			_value = value.ChangeType<Int64>();

			random = new Random((int)randomSeed);
			int max = random.Next(1, 100);

			// TODO - use lightweight dynamic method here?
			for (int i = 0; i < max; i++)
			{
				op = (Op)random.Next(1, 4);

				val = random.Next(); // unbounded

				switch (op)
				{
					case Op.Add:
						_value += val;
						break;
					case Op.Sub:
						_value -= val;
						break;
					case Op.Mul:
						_value *= val;
						break;
					case Op.Div:
						if (val != 0)
							_value /= val;
						break;
					case Op.Mod:
						_value %= val;
						break;
					default:
						break;
				}
			}

			value = _value.ChangeType(valueType);

			await Task.CompletedTask;
			return value;
		}

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

			valueHash = await obfuscationContext.GetValueHashAsync(null, field.FieldName, cancellationToken);
			randomSeed = valueHash;

			// create a new repeatable yet random-ish math function using the random seed, then executes for column value 
			value = await GetSurrogateKeyAsync(randomSeed, originalFieldValue, cancellationToken);

			return value;
		}

		#endregion
	}
}