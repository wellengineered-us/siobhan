/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace WellEngineered.Siobhan.Textual.Fixed
{
	public interface IFixedTextualFieldSpec : ITextualFieldSpec
	{
		#region Properties/Indexers/Events

		long FieldLength
		{
			get;
		}

		long StartPosition
		{
			get;
		}

		#endregion
	}
}