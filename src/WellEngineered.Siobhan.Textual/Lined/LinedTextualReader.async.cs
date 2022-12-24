/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.Textual.Lined
{
	public partial class LinedTextualReader
		: TextualReader<ILinedTextualFieldSpec,
			ILinedTextualSpec>
	{
		#region Methods/Operators

		protected override IAsyncLifecycleEnumerable<ITextualStreamingRecord> CoreReadFooterRecordsAsync(ILifecycleEnumerable<ILinedTextualFieldSpec> footers, CancellationToken cancellationToken = default)
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

		protected override IAsyncLifecycleEnumerable<ILinedTextualFieldSpec> CoreReadHeaderFieldsAsync(CancellationToken cancellationToken = default)
		{
			return GetHeadersAsync(cancellationToken).ToAsyncLifecycleEnumerable();

			async IAsyncEnumerable<ILinedTextualFieldSpec> GetHeadersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				foreach (ILinedTextualFieldSpec linedTextualFieldSpec in new ILinedTextualFieldSpec[] { })
				{
					yield return linedTextualFieldSpec;
				}

				await Task.CompletedTask;
			}
		}

		protected override IAsyncLifecycleEnumerable<ITextualStreamingRecord> CoreReadRecordsAsync(CancellationToken cancellationToken = default)
		{
			return this.ResumableParserMainLoopAsync(false, cancellationToken).ToAsyncLifecycleEnumerable();
		}
		
		private async IAsyncEnumerable<ITextualStreamingRecord> ResumableParserMainLoopAsync(bool yieldOnlyOnce, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			const int EOF = 0; // ReadAsync() ONLY
			const int DEFAULT_INDEX = -1;
			string line;
			
			ITextualStreamingRecord record;
			bool isEndOfFile = false;
			long recordIndex = DEFAULT_INDEX;
			long lineIndex = DEFAULT_INDEX;
			long characterIndexStart = DEFAULT_INDEX;
			long characterIndexEnd = DEFAULT_INDEX;

			StringBuilder recordStringBuilder;

			char previous = '\0';
			
			this.TextualSpec.AssertValid();

			recordStringBuilder = new StringBuilder();

			//await Task.CompletedTask;
			
			// main loop - character stream
			while (!isEndOfFile)
			{
				int __value;
				char current, next;
				
				Memory<char> memory;
				
				memory = new Memory<char>(new char[1]);
				
				// read the next byte
				//__value = this.BaseTextReader.Read();
				//current = (char)__value;
				
				__value = await this.BaseTextReader.ReadAsync(memory, cancellationToken);
				current = memory.Length == 1 && __value == 1 ? memory.Span[0] : '\0';
				
				// check for -1 (EOF)
				if (__value == EOF)
				{
					// set terminal state
					isEndOfFile = true;
				}
				
				// peek the next byte
				__value = this.BaseTextReader.Peek(); // NO ASYNC SUPPORT
				next = (char)__value;

				if (this.TextualSpec.NewLineStyle == NewLineStyle.Auto ||
					this.TextualSpec.NewLineStyle == NewLineStyle.Windows)
				{
					// check parser state
					if ((previous == '\r' && current == '\n'))
					{
						// eat this
						continue;
					}
				}

				// check parser state
				if (isEndOfFile ||
					((this.TextualSpec.NewLineStyle == NewLineStyle.Auto ||
					this.TextualSpec.NewLineStyle == NewLineStyle.Unix)
					&& current == '\n') ||
					((this.TextualSpec.NewLineStyle == NewLineStyle.Auto ||
					this.TextualSpec.NewLineStyle == NewLineStyle.Windows) &&
					current == '\r' && next == '\n'))
				{
					line = recordStringBuilder.ToString();

					// check for blank line (ignore)
					if (line.Length != 0)
					{
						// advance the character index
						characterIndexStart = characterIndexEnd + 1;
						characterIndexEnd = characterIndexStart + (line.Length - 1) +
											((this.TextualSpec.NewLineStyle == NewLineStyle.Auto ||
											this.TextualSpec.NewLineStyle == NewLineStyle.Unix)
											&& current == '\n' ? 1 : 
												(this.TextualSpec.NewLineStyle == NewLineStyle.Auto ||
												this.TextualSpec.NewLineStyle == NewLineStyle.Windows) && 
												current == '\r' && next == '\n' ? 2 : 0);

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
#endif