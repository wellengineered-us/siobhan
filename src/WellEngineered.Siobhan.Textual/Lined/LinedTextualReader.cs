/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.Textual.Lined
{
	public partial class LinedTextualReader
		: TextualReader<ILinedTextualFieldSpec,
			ILinedTextualSpec>
	{
		#region Constructors/Destructors

		public LinedTextualReader(TextReader baseTextReader, ILinedTextualSpec linedTextualSpec)
			: base(baseTextReader, linedTextualSpec)
		{
			this.ResetParserState();
		}

		#endregion

		#region Fields/Constants

		private readonly _ParserState parserState = new _ParserState();

		#endregion

		#region Properties/Indexers/Events

		private _ParserState ParserState
		{
			get
			{
				return this.parserState;
			}
		}

		#endregion

		#region Methods/Operators

		protected override ILifecycleEnumerable<ITextualStreamingRecord> CoreReadFooterRecords(ILifecycleEnumerable<ILinedTextualFieldSpec> footers)
		{
			return GetFooters().ToLifecycleEnumerable();

			IEnumerable<ITextualStreamingRecord> GetFooters()
			{
				foreach (ITextualStreamingRecord textualStreamingRecord in new ITextualStreamingRecord[] { })
				{
					yield return textualStreamingRecord;
				}
			}
		}

		protected override ILifecycleEnumerable<ILinedTextualFieldSpec> CoreReadHeaderFields()
		{
			return GetHeaders().ToLifecycleEnumerable();

			IEnumerable<ILinedTextualFieldSpec> GetHeaders()
			{
				foreach (ILinedTextualFieldSpec linedTextualFieldSpec in new ILinedTextualFieldSpec[] { })
				{
					yield return linedTextualFieldSpec;
				}
			}
		}

		protected override ILifecycleEnumerable<ITextualStreamingRecord> CoreReadRecords()
		{
			return GetRecords().ToLifecycleEnumerable();

			IEnumerable<ITextualStreamingRecord> GetRecords()
			{
				string line;
				ITextualStreamingRecord record;

				while (true)
				{
					line = this.BaseTextReader.ReadLine();

					if (string.IsNullOrEmpty(line))
						yield break;

					// TODO fix counts
					record = new TextualStreamingRecord(this.ParserState.lineIndex, this.ParserState.lineIndex, this.ParserState.characterIndex);
					record.Add(string.Empty, line);

					yield return record;
				}
			}
		}

		private void ResetParserState()
		{
			const long DEFAULT_INDEX = 0;

			this.ParserState.record = new TextualStreamingRecord(0, 0, 0);
			this.ParserState.characterIndex = DEFAULT_INDEX;
			this.ParserState.lineIndex = DEFAULT_INDEX;

			this.TextualSpec.AssertValid();
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private sealed class _ParserState
		{
			#region Fields/Constants

			public long characterIndex;
			public long lineIndex;
			public ITextualStreamingRecord record;

			#endregion
		}

		#endregion
	}
}