/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

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

		public static async ValueTask ForceAsyncEnumeration<T>(this IAsyncEnumerable<T> asyncEnumerable,
			CancellationToken cancellationToken = default)
		{
			if ((object)asyncEnumerable == null)
				throw new ArgumentNullException(nameof(asyncEnumerable));

			await foreach (T item in asyncEnumerable.WithCancellation(cancellationToken))
			{
				// do nothing
			}
		}

		public static async IAsyncEnumerable<TItem> GetWrappedAsyncEnumerable<TItem>(this IAsyncEnumerable<TItem> asyncEnumerable,
			string sourceLabel, Func<long, TItem, ValueTask<TItem>> itemCallback,
			Func<string, long, bool, double, ValueTask> processingCallback,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			long itemIndex = 0;
			DateTime startUtc = DateTime.UtcNow;

			if ((object)asyncEnumerable == null)
				throw new ArgumentNullException(nameof(asyncEnumerable));

			if ((object)processingCallback != null)
				await processingCallback(sourceLabel, -1, false, (DateTime.UtcNow - startUtc).TotalSeconds);

			await foreach (TItem item in asyncEnumerable.WithCancellation(cancellationToken))
			{
				if ((itemIndex % PROCESSING_CALLBACK_WINDOW_SIZE) == 0)
				{
					if ((object)processingCallback != null)
						await processingCallback(sourceLabel, itemIndex, false, (DateTime.UtcNow - startUtc).TotalSeconds);
				}

				if ((object)itemCallback != null)
					yield return await itemCallback(itemIndex, item);
				else
					yield return item;

				itemIndex++;
			}

			if ((object)processingCallback != null)
				await processingCallback(sourceLabel, itemIndex, true, (DateTime.UtcNow - startUtc).TotalSeconds);
		}

		public static IAsyncEnumerable<TItem> GetWrappedAsyncEnumerable<TItem>(this IAsyncEnumerable<TItem> asyncEnumerable,
			Func<long, TItem, ValueTask<TItem>> itemCallback,
			CancellationToken cancellationToken = default)
		{
			if ((object)asyncEnumerable == null)
				throw new ArgumentNullException(nameof(asyncEnumerable));

			return asyncEnumerable.GetWrappedAsyncEnumerable(null, itemCallback, null, cancellationToken);
		}

		#endregion
	}
}