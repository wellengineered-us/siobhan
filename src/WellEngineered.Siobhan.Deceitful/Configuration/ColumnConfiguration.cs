/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Configuration;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Deceitful.Configuration
{
	public partial class ColumnConfiguration<TObfuscationStrategySpec>
		: UnknownSiobhanConfiguration<TObfuscationStrategySpec>
		where TObfuscationStrategySpec : class, ISiobhanSpecification, new()
	{
		#region Constructors/Destructors

		public ColumnConfiguration(IUnknownSolderConfiguration that)
			: base(that)
		{
		}

		#endregion
	}

	public partial class ColumnConfiguration
		: UnknownSiobhanConfiguration
	{
		#region Constructors/Destructors

		public ColumnConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private string columnName;

		#endregion

		#region Properties/Indexers/Events

		public string ColumnName
		{
			get
			{
				return this.columnName;
			}
			set
			{
				this.columnName = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override IEnumerable<IMessage> CoreValidate(object context)
		{
			yield break;
		}

		#endregion
	}
}