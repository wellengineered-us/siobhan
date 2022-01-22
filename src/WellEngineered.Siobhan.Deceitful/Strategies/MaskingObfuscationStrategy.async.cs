/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns an alternate value that is a +/- (%) mask of the original value.
	/// DATA TYPE: string
	/// </summary>
	public sealed partial class MaskingObfuscationStrategy : ObfuscationStrategy<MaskingObfuscationStrategy.Spec>
	{
		#region Methods/Operators

		private static async ValueTask<object> GetMaskAsync(double maskFactor, object value, CancellationToken cancellationToken = default)
		{
			StringBuilder buffer;
			Type valueType;
			string _value;

			if ((int)(maskFactor * 100) > 100)
				throw new ArgumentOutOfRangeException(nameof(maskFactor));

			if ((int)(maskFactor * 100) == 000)
				throw new ArgumentOutOfRangeException(nameof(maskFactor));

			if ((int)(maskFactor * 100) < -100)
				throw new ArgumentOutOfRangeException(nameof(maskFactor));

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (string.IsNullOrWhiteSpace(_value))
				return _value;

			_value = _value.Trim();

			buffer = new StringBuilder(_value);

			if (Math.Sign(maskFactor) == 1)
			{
				for (int index = 0; index < (int)Math.Round((double)_value.Length * maskFactor); index++)
					buffer[index] = '*';
			}
			else if (Math.Sign(maskFactor) == -1)
			{
				for (int index = _value.Length - (int)Math.Round((double)_value.Length * Math.Abs(maskFactor)); index < _value.Length; index++)
					buffer[index] = '*';
			}
			else
				throw new InvalidOperationException("maskFactor");

			await Task.CompletedTask;
			return buffer.ToString();
		}

		protected override async ValueTask<object> CoreGetObfuscatedValueAsync(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken)
		{
			object value;
			double maskingFactor;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			maskingFactor = (this.Specification.MaskingPercentValue.GetValueOrDefault() / 100.0);

			value = await GetMaskAsync(maskingFactor, originalFieldValue, cancellationToken);

			return value;
		}

		#endregion
	}
}