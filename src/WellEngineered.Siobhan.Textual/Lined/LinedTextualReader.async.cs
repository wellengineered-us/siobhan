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
			return GetRecordsAsync(cancellationToken).ToAsyncLifecycleEnumerable();

			async IAsyncEnumerable<ITextualStreamingRecord> GetRecordsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				string line;
				ITextualStreamingRecord record;

				while (!cancellationToken.IsCancellationRequested)
				{
					line = await this.BaseTextReader.ReadLineAsync();

					if (string.IsNullOrEmpty(line))
						yield break;

					// TODO fix counts
					record = new TextualStreamingRecord(this.ParserState.lineIndex, this.ParserState.lineIndex, this.ParserState.characterIndex);
					record.Add(string.Empty, line);

					yield return record;
				}
			}
		}

		#endregion
	}
}
#endif