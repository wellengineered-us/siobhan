/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Threading;

namespace WellEngineered.Siobhan.Primitives
{
	public static partial class EnumerableExtensions
	{
		#region Methods/Operators

		public static IAsyncLifecycleEnumerable<TItem> ToAsyncLifecycleEnumerable<TItem>(this IAsyncEnumerable<TItem> asyncEnumerable,
			CancellationToken cancellationToken = default)
		{
			if ((object)asyncEnumerable == null)
				throw new ArgumentNullException(nameof(asyncEnumerable));

			return new AsyncForEachLifecycleYieldStateMachine<TItem, TItem>(asyncEnumerable, null);
		}

		#endregion
	}
}
#endif