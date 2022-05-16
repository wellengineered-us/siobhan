/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public abstract partial class TextualWriter<TTextualFieldSpec, TTextualSpec>
		: DualLifecycle,
			ITextualWriter<TTextualFieldSpec, TTextualSpec>
		where TTextualFieldSpec : ITextualFieldSpec
		where TTextualSpec : ITextualSpec<TTextualFieldSpec>
	{
		#region Methods/Operators

		protected override ValueTask CoreCreateAsync(bool creating, CancellationToken cancellationToken = default)
		{
			// do nothing
			return default;
		}

		protected override async ValueTask CoreDisposeAsync(bool disposing, CancellationToken cancellationToken = default)
		{
			if (disposing)
				await this.BaseTextWriter.DisposeAsync();
		}

		protected virtual async ValueTask CoreFlushAsync(CancellationToken cancellationToken = default)
		{
			await this.BaseTextWriter.FlushAsync();
		}

		protected abstract ValueTask CoreWriteFooterRecordsAsync(IAsyncLifecycleEnumerable<TTextualFieldSpec> specs, IAsyncLifecycleEnumerable<ITextualStreamingRecord> footers, CancellationToken cancellationToken = default);

		protected abstract ValueTask CoreWriteHeaderFieldsAsync(IAsyncLifecycleEnumerable<TTextualFieldSpec> headers, CancellationToken cancellationToken = default);

		protected abstract ValueTask CoreWriteRecordsAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> records, CancellationToken cancellationToken = default);

		public ValueTask FlushAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				return this.CoreFlushAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual writer failed (see inner exception)."), ex);
			}
		}

		public ValueTask WriteFooterRecordsAsync(IAsyncLifecycleEnumerable<TTextualFieldSpec> specs, IAsyncLifecycleEnumerable<ITextualStreamingRecord> footers, CancellationToken cancellationToken = default)
		{
			if ((object)specs == null)
				throw new ArgumentNullException(nameof(specs));

			if ((object)footers == null)
				throw new ArgumentNullException(nameof(footers));

			try
			{
				return this.CoreWriteFooterRecordsAsync(specs, footers, cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual writer failed (see inner exception)."), ex);
			}
		}

		public ValueTask WriteHeaderFieldsAsync(IAsyncLifecycleEnumerable<TTextualFieldSpec> headers, CancellationToken cancellationToken = default)
		{
			if ((object)headers == null)
				throw new ArgumentNullException(nameof(headers));

			try
			{
				return this.CoreWriteHeaderFieldsAsync(headers, cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual writer failed (see inner exception)."), ex);
			}
		}

		public ValueTask WriteRecordsAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> records, CancellationToken cancellationToken = default)
		{
			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			try
			{
				return this.CoreWriteRecordsAsync(records, cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual writer failed (see inner exception)."), ex);
			}
		}

		#endregion
	}
}
#endif