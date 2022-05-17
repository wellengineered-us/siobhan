/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace WellEngineered.Siobhan.Model
{
	public interface ISiobhanKeyValueView
	{
		#region Properties/Indexers/Events

		ISiobhanPayload KeyPayload
		{
			get;
		}

		ISiobhanSchema KeySchema
		{
			get;
		}

		ISiobhanPayload OriginalPayload
		{
			get;
		}

		ISiobhanSchema OriginalSchema
		{
			get;
		}

		ISiobhanPayload ValuePayload
		{
			get;
		}

		ISiobhanSchema ValueSchema
		{
			get;
		}

		#endregion
	}
}