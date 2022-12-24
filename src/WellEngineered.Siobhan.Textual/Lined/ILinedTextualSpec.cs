/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace WellEngineered.Siobhan.Textual.Lined
{
	public interface ILinedTextualSpec
		: ITextualSpec<ILinedTextualFieldSpec>
	{
		NewLineStyle NewLineStyle
		{
			get;
		}
	}
}