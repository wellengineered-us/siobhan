/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using System.Threading;

using WellEngineered.Solder.Configuration;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Primitives.Configuration
{
	public abstract partial class SiobhanSpecification
		: SolderSpecification,
			ISiobhanSpecification
	{
		#region Methods/Operators

		protected override IAsyncEnumerable<IMessage> CoreValidateAsync(object context, CancellationToken cancellationToken = new CancellationToken())
		{
			return null;
		}

		#endregion
	}
}