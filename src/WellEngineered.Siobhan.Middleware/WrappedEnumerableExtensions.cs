/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Middleware
{
	public static partial class WrappedEnumerableExtensions
	{
		#region Fields/Constants

		private const int PROCESSING_CALLBACK_WINDOW_SIZE = 1000;

		#endregion

		#region Methods/Operators

		public static void ForceEnumeration<T>(this IEnumerable<T> enumerable)
		{
			if ((object)enumerable == null)
				throw new ArgumentNullException(nameof(enumerable));

			foreach (T item in enumerable)
			{
				// do nothing
			}
		}

		public static IEnumerable<TItem> GetWrappedEnumerable<TItem>(this IEnumerable<TItem> enumerable, string sourceLabel, Func<long, TItem, TItem> itemCallback, Action<string, long, bool, double> processingCallback)
		{
			long itemIndex = 0;
			DateTime startUtc = DateTime.UtcNow;

			if ((object)enumerable == null)
				throw new ArgumentNullException(nameof(enumerable));

			if ((object)processingCallback != null)
				processingCallback(sourceLabel, -1, false, (DateTime.UtcNow - startUtc).TotalSeconds);

			foreach (TItem item in enumerable)
			{
				if ((itemIndex % PROCESSING_CALLBACK_WINDOW_SIZE) == 0)
				{
					if ((object)processingCallback != null)
						processingCallback(sourceLabel, itemIndex, false, (DateTime.UtcNow - startUtc).TotalSeconds);
				}

				if ((object)itemCallback != null)
					yield return itemCallback(itemIndex, item);
				else
					yield return item;

				itemIndex++;
			}

			if ((object)processingCallback != null)
				processingCallback(sourceLabel, itemIndex, true, (DateTime.UtcNow - startUtc).TotalSeconds);
		}

		public static IEnumerable<TItem> GetWrappedEnumerable<TItem>(this IEnumerable<TItem> enumerable, Func<long, TItem, TItem> itemCallback)
		{
			return enumerable.GetWrappedEnumerable(null, itemCallback, null);
		}

		#endregion
	}
}