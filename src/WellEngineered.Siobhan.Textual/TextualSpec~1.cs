/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Textual
{
	public abstract class TextualSpec<TTextualFieldSpec> : ITextualSpec<TTextualFieldSpec>
		where TTextualFieldSpec : ITextualFieldSpec
	{
		#region Constructors/Destructors

		protected TextualSpec()
			: this(new List<TTextualFieldSpec>())
		{
		}

		protected TextualSpec(IList<TTextualFieldSpec> textualHeaderSpecs)
		{
			if ((object)textualHeaderSpecs == null)
				throw new ArgumentNullException(nameof(textualHeaderSpecs));

			this.textualHeaderSpecs = textualHeaderSpecs;
		}

		#endregion

		#region Fields/Constants

		private readonly IList<TTextualFieldSpec> textualHeaderSpecs;
		private string contentEncoding;

		#endregion

		#region Properties/Indexers/Events

		public IList<TTextualFieldSpec> HeaderSpecs
		{
			get
			{
				return this.textualHeaderSpecs;
			}
		}

		public string ContentEncoding
		{
			get
			{
				return this.contentEncoding;
			}
			set
			{
				this.contentEncoding = value;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void AssertValid()
		{
			// do nothing
		}

		#endregion
	}
}