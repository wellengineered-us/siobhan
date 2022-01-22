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
	/// Returns un-obfuscated, original value.
	/// DATA TYPE: any
	/// </summary>
	public sealed partial class SurrogateKeyObfuscationStrategy : ObfuscationStrategy<SurrogateKeyObfuscationStrategy.Spec>
	{
		#region Constructors/Destructors

		public SurrogateKeyObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static object GetSurrogateKey(long randomSeed, object value)
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

			// TODO - use lighweight dynmamic method here?
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
			return value;
		}

		protected override object CoreGetObfuscatedValue(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue)
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

			valueHash = obfuscationContext.GetValueHash(null, field.FieldName);
			randomSeed = valueHash;

			// create a new repeatable yet random-ish math function using the random seed, then executes for column value 
			value = GetSurrogateKey(randomSeed, originalFieldValue);

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
		}

		private enum Op
		{
			Add = 1,
			Sub,
			Mul,
			Div,
			Mod
		}

		#endregion
	}
}