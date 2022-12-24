/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace WellEngineered.Siobhan.Textual.Lined
{
	public class LinedTextualSpec : TextualSpec<ILinedTextualFieldSpec>, ILinedTextualSpec
	{
		#region Constructors/Destructors

		public LinedTextualSpec()
		{
		}

		#endregion

		private NewLineStyle newLineStyle;

		public NewLineStyle NewLineStyle
		{
			get
			{
				return this.newLineStyle;
			}
			set
			{
				this.newLineStyle = value;
			}
		}
	}
}