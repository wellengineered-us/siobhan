/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace WellEngineered.Siobhan.Textual
{
	public interface ITextualSpec<TTextualFieldSpec>
		where TTextualFieldSpec : ITextualFieldSpec
	{
		#region Properties/Indexers/Events

		bool IsFirstRecordHeader
		{
			get;
		}

		bool IsLastRecordFooter
		{
			get;
		}

		string RecordDelimiter
		{
			get;
		}

		IList<TTextualFieldSpec> TextualFooterSpecs
		{
			get;
		}

		IList<TTextualFieldSpec> TextualHeaderSpecs
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void AssertValid();

		#endregion
	}
}