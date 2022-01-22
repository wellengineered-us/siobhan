/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System.Threading;

using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public partial interface ITextualReader<TTextualFieldSpec, out TTextualSpec>
		: IAsyncLifecycle
		where TTextualFieldSpec : ITextualFieldSpec
		where TTextualSpec : ITextualSpec<TTextualFieldSpec>
	{
		#region Methods/Operators

		IAsyncLifecycleEnumerable<ITextualStreamingRecord> ReadFooterRecordsAsync(ILifecycleEnumerable<TTextualFieldSpec> footers, CancellationToken cancellationToken = default);

		IAsyncLifecycleEnumerable<TTextualFieldSpec> ReadHeaderFieldsAsync(CancellationToken cancellationToken = default);

		IAsyncLifecycleEnumerable<ITextualStreamingRecord> ReadRecordsAsync(CancellationToken cancellationToken = default);

		#endregion
	}
}
#endif