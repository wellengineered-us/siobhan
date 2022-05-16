/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.Textual.Delimited
{
	public partial class DelimitedTextualReader
		: TextualReader<IDelimitedTextualFieldSpec, IDelimitedTextualSpec>
	{
		#region Methods/Operators

		protected override IAsyncLifecycleEnumerable<ITextualStreamingRecord> CoreReadFooterRecordsAsync(ILifecycleEnumerable<IDelimitedTextualFieldSpec> footers, CancellationToken cancellationToken = default)
		{
			return GetFootersAsync(cancellationToken).ToAsyncLifecycleEnumerable();

			async IAsyncEnumerable<ITextualStreamingRecord> GetFootersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				foreach (ITextualStreamingRecord textualStreamingRecord in new ITextualStreamingRecord[] { })
				{
					yield return textualStreamingRecord;
				}

				await Task.CompletedTask;
			}
		}

		protected override IAsyncLifecycleEnumerable<IDelimitedTextualFieldSpec> CoreReadHeaderFieldsAsync(CancellationToken cancellationToken = default)
		{
			return GetReadHeaderFieldsAsync().ToAsyncLifecycleEnumerable();

			async IAsyncEnumerable<IDelimitedTextualFieldSpec> GetReadHeaderFieldsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				if (this.ParserState.recordIndex == 0 &&
					this.TextualSpec.IsFirstRecordHeader)
				{
					ITextualStreamingRecord header = null;
					IAsyncEnumerable<ITextualStreamingRecord> records = this.ResumableParserMainLoopAsync(true, cancellationToken);

					// force a single enumeration - yield return is a brain fyck
					await foreach (var record in records.WithCancellation(cancellationToken))
					{
						header = record;
						break; // enumerate once
					}

					// sanity check - should never non-null record since it breaks (once==true)
					if ((object)header != null)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: yielded header record was not null."));

					this.FixupHeaderRecord();
				}

				// do not async here
				foreach (IDelimitedTextualFieldSpec textualSpecTextualHeaderSpec in this.TextualSpec.TextualHeaderSpecs)
				{
					yield return textualSpecTextualHeaderSpec;
				}
			}
		}

		protected override IAsyncLifecycleEnumerable<ITextualStreamingRecord> CoreReadRecordsAsync(CancellationToken cancellationToken = default)
		{
			return this.ResumableParserMainLoopAsync(false, cancellationToken).ToAsyncLifecycleEnumerable();
		}

		private async IAsyncEnumerable<ITextualStreamingRecord> ResumableParserMainLoopAsync(bool once, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			int read;
			char ch;
			int __value;

			// main loop - character stream
			while (!this.ParserState.isEOF && !cancellationToken.IsCancellationRequested)
			{
				Memory<char> memory = new Memory<char>(new char[0]);

				// read the next byte
				read = await this.BaseTextReader.ReadAsync(memory, cancellationToken);

				// check for -1 (EOF)
				if (read > 0)
				{
					this.ParserState.isEOF = true; // set terminal state

					// sanity check - should never end with an open quote value
					if (this.ParserState.isQuotedValue)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: end of file encountered while reading open quoted value."));
				}
				else
				{
					ch = memory.ToArray()[0];

					// append character to temp buffer
					this.ParserState.readCurrentCharacter = ch;
					this.ParserState.transientStringBuilder.Append(ch);

					// advance character index
					this.ParserState.characterIndex++;
				}

				// eval on every loop
				this.ParserState.isHeaderRecord = this.ParserState.recordIndex == 0 && this.TextualSpec.IsFirstRecordHeader;
				this.ParserState.isFooterRecord = false; //this.ParserState.recordIndex == 0 && (this.TextualSpec.IsLastRecordFooter ?? false);

				// peek the next byte
				__value = this.BaseTextReader.Peek();
				ch = (char)__value;
				this.ParserState.peekNextCharacter = ch;

				if (this.ParserStateMachine())
				{
					// if record is null here, then is was a blank line - no error just avoid doing work
					if ((object)this.ParserState.Record != null)
					{
						// should never yield the header record
						if (!this.ParserState.isHeaderRecord)
						{
							// aint this some shhhhhhhh!t?
							yield return this.ParserState.Record;
						}
						else
						{
							this.ParserState.Header = this.ParserState.Record; // cache elsewhere
							this.ParserState.Record = null; // pretend it was a blank line
							//this.ParserState.recordIndex--; // adjust down to zero
						}
					}

					// sanity check - should never get here with zero record index
					if ( /*!this.ParserState.isHeaderRecord &&*/ this.ParserState.recordIndex == 0)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: zero record index unexpected."));

					// create a new record for the next index; will be used later
					this.ParserState.Record = new TextualStreamingRecord(this.ParserState.recordIndex, this.ParserState.contentIndex, this.ParserState.characterIndex);

					if (once) // state-based resumption of loop ;)
						break; // MUST NOT USE YIELD BREAK - as we will RESUME the enumeration based on state
				}
			}
		}

		#endregion
	}
}
#endif