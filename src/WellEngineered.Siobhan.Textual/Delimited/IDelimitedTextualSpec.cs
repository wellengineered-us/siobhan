/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace WellEngineered.Siobhan.Textual.Delimited
{
	public interface IDelimitedTextualSpec
		: ITextualSpec<IDelimitedTextualFieldSpec>
	{
		#region Properties/Indexers/Events

		string CloseQuoteValue
		{
			get;
			set;
		}

		string FieldDelimiter
		{
			get;
			set;
		}

		string OpenQuoteValue
		{
			get;
			set;
		}

		#endregion
	}
}