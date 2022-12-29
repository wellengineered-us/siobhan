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
using WellEngineered.Siobhan.Textual.Lined;

namespace WellEngineered.Siobhan.Textual.Delimited
{
	public partial class DelimitedTextualReader
		: TextualReader<IDelimitedTextualFieldSpec, IDelimitedTextualSpec>
	{
		#region Methods/Operators

		protected override IAsyncLifecycleEnumerable<IDelimitedTextualFieldSpec> CoreReadHeaderFieldsAsync(CancellationToken cancellationToken = default)
		{
			return GetHeadersAsync(cancellationToken).ToAsyncLifecycleEnumerable();

			async IAsyncEnumerable<IDelimitedTextualFieldSpec> GetHeadersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				foreach (IDelimitedTextualFieldSpec linedTextualFieldSpec in new IDelimitedTextualFieldSpec[] { })
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
			yield break;
		}

		#endregion
	}
}
#endif