/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WellEngineered.Siobhan.Middleware
{
	public static partial class WrappedEnumerableExtensions
	{
		#region Methods/Operators

		public static async IAsyncEnumerable<TItem> GetWrappedAsyncEnumerable<TItem>(this IAsyncEnumerable<TItem> asyncEnumerable,
			string sourceLabel, Func<long, TItem, CancellationToken, ValueTask<TItem>> itemCallback,
			Func<string, long, bool, double, CancellationToken, ValueTask> processingCallback,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			long itemIndex = -1;
			DateTime startUtc = DateTime.UtcNow;

			if ((object)asyncEnumerable == null)
				throw new ArgumentNullException(nameof(asyncEnumerable));

			if ((object)processingCallback != null)
				await processingCallback(sourceLabel, itemIndex, false, (DateTime.UtcNow - startUtc).TotalSeconds, cancellationToken);

			await foreach (TItem item in asyncEnumerable.WithCancellation(cancellationToken))
			{
				if ((itemIndex % PROCESSING_CALLBACK_WINDOW_SIZE) == 0)
				{
					if ((object)processingCallback != null)
						await processingCallback(sourceLabel, itemIndex, false, (DateTime.UtcNow - startUtc).TotalSeconds, cancellationToken);
				}

				if ((object)itemCallback != null)
					yield return await itemCallback(itemIndex, item, cancellationToken);
				else
					yield return item;

				itemIndex++;
			}

			if ((object)processingCallback != null)
				await processingCallback(sourceLabel, itemIndex, true, (DateTime.UtcNow - startUtc).TotalSeconds, cancellationToken);
		}

		public static IAsyncEnumerable<TItem> GetWrappedAsyncEnumerable<TItem>(this IAsyncEnumerable<TItem> asyncEnumerable,
			Func<long, TItem, CancellationToken, ValueTask<TItem>> itemCallback,
			CancellationToken cancellationToken = default)
		{
			if ((object)asyncEnumerable == null)
				throw new ArgumentNullException(nameof(asyncEnumerable));

			return asyncEnumerable.GetWrappedAsyncEnumerable(null, itemCallback, null, cancellationToken);
		}

		#endregion
	}
}
#endif