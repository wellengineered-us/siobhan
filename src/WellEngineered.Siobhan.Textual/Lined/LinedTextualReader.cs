/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
		}

		#endregion

		#region Methods/Operators

		protected override ILifecycleEnumerable<ILinedTextualFieldSpec> CoreReadHeaderFields()
		{
			return GetHeaders().ToLifecycleEnumerable();

			IEnumerable<ILinedTextualFieldSpec> GetHeaders()
			{
				yield break;
			}
		}

		protected override ILifecycleEnumerable<ITextualStreamingRecord> CoreReadRecords()
		{
			return this.ResumableParserMainLoop(false).ToLifecycleEnumerable();
		}

		private IEnumerable<ITextualStreamingRecord> ResumableParserMainLoop(bool yieldOnlyOnce)
		{
			const char EOL_UNIX = '\n';
			const char EOL_WIN_A = '\r';
			const char EOL_WIN_B = '\n';
			
			const string EOL_UNIX_STR = "\n";
			const string EOL_WIN_STR = "\r\n";
			
			const int EOF = -1;
			const int DEFAULT_INDEX = -1;
			string line;
			
			ITextualStreamingRecord record;
			bool isEndOfFile = false;
			long recordIndex = DEFAULT_INDEX;
			long lineIndex = DEFAULT_INDEX;
			long characterIndexStart = DEFAULT_INDEX;
			long characterIndexEnd = DEFAULT_INDEX;

			StringBuilder recordStringBuilder;

			int buffer;
			char current, next;
			char previous = '\0';

			// [value] [newline(\n|\r\n)] | [eof]
			
			this.TextualSpec.AssertValid();

			recordStringBuilder = new StringBuilder();

			// main loop - character stream
			while (!isEndOfFile)
			{
				// read the next byte
				buffer = this.BaseTextReader.Read();
				current = (char)buffer;
				
				// check for -1 (EOF)
				if (buffer == EOF)
				{
					// set terminal state
					isEndOfFile = true;
				}
				
				// peek the next byte
				buffer = this.BaseTextReader.Peek();
				next = (char)buffer;

				if ((this.TextualSpec.NewLineStyle == NewLineStyle.Auto && Environment.NewLine == EOL_WIN_STR) ||
					this.TextualSpec.NewLineStyle == NewLineStyle.Windows)
				{
					// check parser state
					if (previous == EOL_WIN_A && current == EOL_WIN_B)
					{
						// eat this
						continue;
					}
				}

				// check parser state
				if (isEndOfFile ||
					(((this.TextualSpec.NewLineStyle == NewLineStyle.Auto && Environment.NewLine == EOL_UNIX_STR) ||
					this.TextualSpec.NewLineStyle == NewLineStyle.Unix)
						&& current == EOL_UNIX) ||
					(((this.TextualSpec.NewLineStyle == NewLineStyle.Auto && Environment.NewLine == EOL_WIN_STR) ||
					this.TextualSpec.NewLineStyle == NewLineStyle.Windows) &&
						current == EOL_WIN_A && next == EOL_WIN_B))
				{
					line = recordStringBuilder.ToString();

					// check for blank line (ignore)
					if (line.Length != 0)
					{
						// advance the character index
						characterIndexStart = characterIndexEnd + 1;
						characterIndexEnd = characterIndexStart + (line.Length - 1) +
											(((this.TextualSpec.NewLineStyle == NewLineStyle.Auto && Environment.NewLine == EOL_UNIX_STR) ||
											this.TextualSpec.NewLineStyle == NewLineStyle.Unix)
												&& current == EOL_UNIX ? 1 : 
												((this.TextualSpec.NewLineStyle == NewLineStyle.Auto && Environment.NewLine == EOL_WIN_STR) ||
												this.TextualSpec.NewLineStyle == NewLineStyle.Windows) && 
													current == EOL_WIN_A && next == EOL_WIN_B ? 2 : 0);

						// advance record index
						recordIndex++;

						// advance line index
						lineIndex++;

						// create this yielding record; (indices are zero-based; numbers are one's based)
						record = new TextualStreamingRecord(recordIndex, lineIndex + 1, characterIndexStart + 1, characterIndexEnd + 1);

						// use default field name (key) and commit value to record
						record.Add(string.Empty, line);

						// ain't this some shhhhhhhh!t?
						yield return record;
					}

					if (yieldOnlyOnce) // state-based resumption of loop ;)
						break; // MUST NOT USE YIELD BREAK - as we will RESUME the enumeration based on state

					// continue resumable parser loop upon next enumeration :>
					
					recordStringBuilder.Clear();
				}
				else
				{
					recordStringBuilder.Append(current);
				}

				previous = current;
			}
		}

		#endregion
	}
}