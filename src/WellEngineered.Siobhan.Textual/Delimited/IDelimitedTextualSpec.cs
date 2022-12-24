/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace WellEngineered.Siobhan.Textual.Delimited
{
	public interface IDelimitedTextualSpec
		: ITextualSpec<IDelimitedTextualFieldSpec>
	{
		#region Properties/Indexers/Events
		
		int SkipInitialRecordCount
		{
			get;
		}
		
		bool IsFirstRecordHeader
		{
			get;
		}

		string RecordDelimiter
		{
			get;
		}
		
		string CloseQuoteValue
		{
			get;
		}

		string FieldDelimiter
		{
			get;
		}

		string OpenQuoteValue
		{
			get;
		}

		#endregion
	}
}