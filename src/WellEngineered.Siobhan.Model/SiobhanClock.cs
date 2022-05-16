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

		public SiobhanClock()
		{
		}

		#endregion

		#region Fields/Constants

		private long? relativeIndex;
		private DateTime? wallTimeUtc;

		#endregion

		#region Properties/Indexers/Events

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
			set
			{
				this.wallTimeUtc = value;
			}
		}

		#endregion
	}
}