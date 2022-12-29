/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public partial interface ITextualWriter<in TTextualFieldSpec, out TTextualSpec>
		: IAsyncLifecycle
		where TTextualFieldSpec : ITextualFieldSpec
		where TTextualSpec : ITextualSpec<TTextualFieldSpec>
	{
		#region Methods/Operators

		ValueTask FlushAsync(CancellationToken cancellationToken = default);

		ValueTask WriteHeaderFieldsAsync(IAsyncLifecycleEnumerable<TTextualFieldSpec> specs, CancellationToken cancellationToken = default);

		ValueTask WriteRecordsAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> records, CancellationToken cancellationToken = default);

		#endregion
	}
}
#endif