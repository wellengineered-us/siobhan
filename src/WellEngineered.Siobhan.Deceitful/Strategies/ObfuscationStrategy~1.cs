/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Siobhan.Primitives.Component;
using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Configuration;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	public abstract partial class ObfuscationStrategy<TObfuscationStrategySpec>
		: SiobhanComponent<UnknownSiobhanConfiguration<TObfuscationStrategySpec>, TObfuscationStrategySpec>,
			IObfuscationStrategy<TObfuscationStrategySpec>
		where TObfuscationStrategySpec : class, ISiobhanSpecification, new()
	{
		#region Constructors/Destructors

		protected ObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		protected sealed override IUnknownSolderConfiguration<TObfuscationStrategySpec> CoreCreateGenericTypedUnknownConfiguration()
		{
			return new UnknownSiobhanConfiguration<TObfuscationStrategySpec>(this.Configuration);
		}

		protected abstract object CoreGetObfuscatedValue(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue);

		public object GetObfuscatedValue(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue)
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
				value = this.CoreGetObfuscatedValue(obfuscationContext, columnConfiguration, field, originalFieldValue);
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