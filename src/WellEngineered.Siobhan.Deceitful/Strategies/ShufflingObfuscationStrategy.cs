/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Text;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives.Configuration;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns an alternate value using a hashed shuffle of alphanumeric characters (while preserving other characters).
	/// DATA TYPE: string
	/// </summary>
	public sealed partial class ShufflingObfuscationStrategy : ObfuscationStrategy<ShufflingObfuscationStrategy.Spec>
	{
		#region Constructors/Destructors

		public ShufflingObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static string FisherYatesShuffle(int seed, string value)
		{
			Random random;
			StringBuilder buffer;
			int index;

			random = new Random(seed);
			buffer = new StringBuilder(value);

			index = buffer.Length;
			while (index > 1)
			{
				int xedni;
				char ch;

				index--;
				xedni = random.Next(index + 1);
				ch = buffer[xedni];
				buffer[xedni] = buffer[index];
				buffer[index] = ch;
			}

			value = buffer.ToString();
			return value;
		}

		private static object GetShuffle(long randomSeed, object value)
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

			return _value;
		}

		private static void ImplDenormalize(Dictionary<int, char> fidelityMap, ref string value)
		{
			StringBuilder sb;
			char ch;
			int offset = 0;

			if ((object)fidelityMap == null)
				throw new ArgumentNullException(nameof(fidelityMap));

			if (string.IsNullOrWhiteSpace(value))
				return;

			sb = new StringBuilder(value);

			for (int index = 0; index < value.Length; index++)
			{
				if (fidelityMap.TryGetValue(index, out ch))
				{
					sb.Insert(index, ch);
					offset++;
				}
			}

			value = sb.ToString();
		}

		private static Dictionary<int, char> ImplNormalize(ref string value)
		{
			StringBuilder sb;
			Dictionary<int, char> fidelityMap;

			fidelityMap = new Dictionary<int, char>();

			if (string.IsNullOrWhiteSpace(value))
				return fidelityMap;

			sb = new StringBuilder();

			// 212-555-1234 => 2125551212 => 1945687302 => 194-568-7302
			for (int index = 0; index < value.Length; index++)
			{
				if (char.IsLetterOrDigit(value[index]))
					sb.Append(value[index]);
				else
					fidelityMap.Add(index, value[index]);
			}

			value = sb.ToString();
			return fidelityMap;
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

			valueHash = obfuscationContext.GetValueHash(null, originalFieldValue);
			randomSeed = valueHash;

			value = GetShuffle(randomSeed, originalFieldValue);

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

		#endregion
	}
}