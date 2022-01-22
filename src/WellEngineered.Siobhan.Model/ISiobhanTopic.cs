/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Model
{
	public interface ISiobhanTopic
	{
		#region Properties/Indexers/Events

		Guid? TopicId
		{
			get;
		}

		string TopicName
		{
			get;
		}

		#endregion
	}
}