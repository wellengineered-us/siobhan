/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Relational
{
	public sealed class AdoNetStreamingResult : IAdoNetStreamingResult
	{
		#region Constructors/Destructors

		public AdoNetStreamingResult(long resultIndex)
		{
			this.resultIndex = resultIndex;
		}

		#endregion

		#region Fields/Constants

		private readonly long resultIndex;
		private IAsyncEnumerable<IAdoNetStreamingRecord> asyncRecords;
		private IEnumerable<IAdoNetStreamingRecord> records;
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

		public IEnumerable<IAdoNetStreamingRecord> Records
		{
			get
			{
				return this.records;
			}
			set
			{
				this.records = value;
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