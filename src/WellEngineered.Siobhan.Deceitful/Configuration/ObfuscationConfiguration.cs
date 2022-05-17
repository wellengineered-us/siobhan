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
	public partial class ObfuscationConfiguration
		: SiobhanConfiguration
	{
		#region Constructors/Destructors

		public ObfuscationConfiguration()
		{
			this.dictionaryConfigurations = new SolderConfigurationCollection<DictionaryConfiguration>(this);
		}

		public ObfuscationConfiguration(ISolderConfigurationCollection<DictionaryConfiguration> dictionaryConfigurations)
		{
			if ((object)dictionaryConfigurations == null)
				throw new ArgumentNullException(nameof(dictionaryConfigurations));

			this.dictionaryConfigurations = dictionaryConfigurations;
		}

		#endregion

		#region Fields/Constants

		private readonly ISolderConfigurationCollection<DictionaryConfiguration> dictionaryConfigurations;

		private bool? disableEngineCaches;
		private bool? enablePassThru;
		private HashConfiguration hashConfiguration;
		private TableConfiguration tableConfiguration;

		#endregion

		#region Properties/Indexers/Events

		public ISolderConfigurationCollection<DictionaryConfiguration> DictionaryConfigurations
		{
			get
			{
				return this.dictionaryConfigurations;
			}
		}

		public bool? DisableEngineCaches
		{
			get
			{
				return this.disableEngineCaches;
			}
			set
			{
				this.disableEngineCaches = value;
			}
		}

		public bool? EnablePassThru
		{
			get
			{
				return this.enablePassThru;
			}
			set
			{
				this.enablePassThru = value;
			}
		}

		public HashConfiguration HashConfiguration
		{
			get
			{
				return this.hashConfiguration;
			}
			set
			{
				this.EnsureParentOnPropertySet(this.hashConfiguration, value);
				this.hashConfiguration = value;
			}
		}

		public TableConfiguration TableConfiguration
		{
			get
			{
				return this.tableConfiguration;
			}
			set
			{
				this.EnsureParentOnPropertySet(this.tableConfiguration, value);
				this.tableConfiguration = value;
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