/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Primitives
{
	public static partial class EnumerableExtensions
	{
		#region Methods/Operators

		public static ILifecycleEnumerable<TItem> ToLifecycleEnumerable<TItem>(this IEnumerable<TItem> enumerable)
		{
			if ((object)enumerable == null)
				throw new ArgumentNullException(nameof(enumerable));

			return new ForEachLifecycleYieldStateMachine<TItem, TItem>(enumerable, null);
		}

		#endregion
	}
}