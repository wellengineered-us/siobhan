/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Model
{
	public interface ISiobhanClock
	{
		#region Properties/Indexers/Events

		long? RelativeIndex
		{
			get;
		}

		DateTime? WallTimeUtc
		{
			get;
		}

		#endregion
	}
}