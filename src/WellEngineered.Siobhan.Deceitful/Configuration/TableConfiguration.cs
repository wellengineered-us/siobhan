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
	public partial class TableConfiguration
		: SiobhanConfiguration
	{
		#region Constructors/Destructors

		public TableConfiguration()
		{
			this.columnConfigurations = new SolderConfigurationCollection<ColumnConfiguration>(this);
		}

		public TableConfiguration(ISolderConfigurationCollection<ColumnConfiguration> columnConfigurations)
		{
			if ((object)columnConfigurations == null)
				throw new ArgumentNullException(nameof(columnConfigurations));

			this.columnConfigurations = columnConfigurations;
		}

		#endregion

		#region Fields/Constants

		private readonly ISolderConfigurationCollection<ColumnConfiguration> columnConfigurations;

		#endregion

		#region Properties/Indexers/Events

		public ISolderConfigurationCollection<ColumnConfiguration> ColumnConfigurations
		{
			get
			{
				return this.columnConfigurations;
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