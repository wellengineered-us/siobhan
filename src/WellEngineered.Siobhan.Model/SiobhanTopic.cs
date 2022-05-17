/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Model
{
	public class SiobhanTopic : ISiobhanTopic
	{
		#region Constructors/Destructors

		public SiobhanTopic()
		{
		}

		#endregion

		#region Fields/Constants

		private Guid? topicId;
		private string topicName;

		#endregion

		#region Properties/Indexers/Events

		public Guid? TopicId
		{
			get
			{
				return this.topicId;
			}
			set
			{
				this.topicId = value;
			}
		}

		public string TopicName
		{
			get
			{
				return this.topicName;
			}
			set
			{
				this.topicName = value;
			}
		}

		#endregion
	}
}