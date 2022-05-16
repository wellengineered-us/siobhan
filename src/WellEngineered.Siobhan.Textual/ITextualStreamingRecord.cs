/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using WellEngineered.Siobhan.Model;

namespace WellEngineered.Siobhan.Textual
{
	public interface ITextualStreamingRecord : ISiobhanPayload
	{
		#region Properties/Indexers/Events

		long CharacterNumber
		{
			get;
		}

		long LineNumber
		{
			get;
		}

		long RecordIndex
		{
			get;
		}

		#endregion
	}
}