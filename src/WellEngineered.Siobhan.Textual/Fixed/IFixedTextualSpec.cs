/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace WellEngineered.Siobhan.Textual.Fixed
{
	public interface IFixedTextualSpec : ITextualSpec<IFixedTextualFieldSpec>
	{
		#region Properties/Indexers/Events

		char FillCharacter
		{
			get;
		}

		long RecordLength
		{
			get;
		}

		bool UsingRecordDelimiter
		{
			get;
		}

		#endregion
	}
}