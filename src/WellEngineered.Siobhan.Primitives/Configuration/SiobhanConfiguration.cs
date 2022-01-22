/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

using WellEngineered.Solder.Configuration;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Primitives.Configuration
{
	public abstract partial class SiobhanConfiguration
		: SolderConfiguration,
			ISiobhanConfiguration
	{
		#region Constructors/Destructors

		protected SiobhanConfiguration()
		{
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