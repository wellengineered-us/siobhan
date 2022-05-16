/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Relational
{
	public sealed class AsyncAdoNetStreamingResult : IAsyncAdoNetStreamingResult
	{
		#region Constructors/Destructors

		public AsyncAdoNetStreamingResult(long resultIndex)
		{
			this.resultIndex = resultIndex;
		}

		#endregion

		#region Fields/Constants

		private readonly long resultIndex;
		private IAsyncEnumerable<IAdoNetStreamingRecord> asyncRecords;
		private int recordsAffected;

		#endregion

		#region Properties/Indexers/Events

		public long ResultIndex
		{
			get
			{
				return this.resultIndex;
			}
		}

		public IAsyncEnumerable<IAdoNetStreamingRecord> AsyncRecords
		{
			get
			{
				return this.asyncRecords;
			}
			set
			{
				this.asyncRecords = value;
			}
		}

		public int RecordsAffected
		{
			get
			{
				return this.recordsAffected;
			}
			set
			{
				this.recordsAffected = value;
			}
		}

		#endregion
	}
}
#endif