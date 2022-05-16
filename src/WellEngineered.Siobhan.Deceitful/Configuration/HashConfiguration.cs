/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Deceitful.Configuration
{
	public partial class HashConfiguration
		: SiobhanConfiguration
	{
		#region Constructors/Destructors

		public HashConfiguration()
		{
		}

		#endregion

		#region Fields/Constants

		private long? multiplier;
		private long? seed;

		#endregion

		#region Properties/Indexers/Events

		public long? Multiplier
		{
			get
			{
				return this.multiplier;
			}
			set
			{
				this.multiplier = value;
			}
		}

		public long? Seed
		{
			get
			{
				return this.seed;
			}
			set
			{
				this.seed = value;
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