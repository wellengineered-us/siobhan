/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Model;

namespace WellEngineered.Siobhan.Relational
{
	public interface IAdoNetStreamingRecord : ISiobhanPayload
	{
		#region Properties/Indexers/Events

		long RecordIndex
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