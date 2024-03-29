/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace WellEngineered.Siobhan.Textual
{
	public interface ITextualSpec<TTextualFieldSpec>
		where TTextualFieldSpec : ITextualFieldSpec
	{
		string ContentEncoding
		{
			get;
		}

		IList<TTextualFieldSpec> HeaderSpecs
		{
			get;
		}
		
		#region Methods/Operators

		void AssertValid();

		#endregion
	}
}