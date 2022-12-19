/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Model
{
	public class SiobhanClock : ISiobhanClock
	{
		#region Constructors/Destructors

		public SiobhanClock(long? relativeIndex, DateTime? wallTimeUtc)
		{
			this.relativeIndex = relativeIndex;
			this.wallTimeUtc = wallTimeUtc;
		}

		#endregion

		#region Fields/Constants

		private long? relativeIndex;
		private readonly DateTime? wallTimeUtc;

		#endregion

		#region Properties/Indexers/Events

		public static ISiobhanClock GetNowAt(long index = -1L)
		{
			return new SiobhanClock(index, DateTime.UtcNow);
		}

		public long? RelativeIndex
		{
			get
			{
				return this.relativeIndex;
			}
			set
			{
				this.relativeIndex = value;
			}
		}

		public DateTime? WallTimeUtc
		{
			get
			{
				return this.wallTimeUtc;
			}
		}

		#endregion
	}
}