/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Deceitful.Configuration
{
	public partial class DictionaryConfiguration
		: SiobhanConfiguration
	{
		#region Constructors/Destructors

		public DictionaryConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private string dictionaryId;
		private bool preloadEnabled;
		private long? recordCount;

		#endregion

		#region Properties/Indexers/Events

		public string DictionaryId
		{
			get
			{
				return this.dictionaryId;
			}
			set
			{
				this.dictionaryId = value;
			}
		}

		public bool PreloadEnabled
		{
			get
			{
				return this.preloadEnabled;
			}
			set
			{
				this.preloadEnabled = value;
			}
		}

		public long? RecordCount
		{
			get
			{
				return this.recordCount;
			}
			set
			{
				this.recordCount = value;
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