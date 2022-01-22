/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Relational
{
	public interface IAdoNetStreamingResult
	{
		#region Properties/Indexers/Events

		IAsyncEnumerable<IAdoNetStreamingRecord> AsyncRecords
		{
			get;
		}

		IEnumerable<IAdoNetStreamingRecord> Records
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