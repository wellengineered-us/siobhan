/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Threading;

using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Deceitful.Configuration
{
	public partial class TableConfiguration
		: SiobhanConfiguration
	{
		#region Methods/Operators

		protected override IAsyncEnumerable<IMessage> CoreValidateAsync(object context, CancellationToken cancellationToken = new CancellationToken())
		{
			return null;
		}

		#endregion
	}
}
#endif