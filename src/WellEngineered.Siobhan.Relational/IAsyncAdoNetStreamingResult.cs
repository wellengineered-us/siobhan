/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Relational
{
	public interface IAsyncAdoNetStreamingResult
	{
		#region Properties/Indexers/Events

		IAsyncEnumerable<IAdoNetStreamingRecord> AsyncRecords
		{
			get;
		}

		int RecordsAffected
		{
			get;
		}

		long ResultIndex
		{
			get;
		}

		#endregion
	}
}
#endif