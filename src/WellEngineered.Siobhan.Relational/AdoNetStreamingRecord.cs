/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Relational
{
	public sealed class AdoNetStreamingRecord : Dictionary<string, object>, IAdoNetStreamingRecord
	{
		#region Constructors/Destructors

		public AdoNetStreamingRecord(long resultIndex, long recordIndex)
			: base(StringComparer.OrdinalIgnoreCase)
		{
			this.resultIndex = resultIndex;
			this.recordIndex = recordIndex;
		}

		#endregion

		#region Fields/Constants

		private readonly long recordIndex;
		private readonly long resultIndex;

		#endregion

		#region Properties/Indexers/Events

		public long RecordIndex
		{
			get
			{
				return this.recordIndex;
			}
		}

		public long ResultIndex
		{
			get
			{
				return this.resultIndex;
			}
		}

		#endregion
	}
}