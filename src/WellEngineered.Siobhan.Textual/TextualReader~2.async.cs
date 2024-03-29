/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public abstract partial class TextualReader<TTextualFieldSpec, TTextualSpec>
		: DualLifecycle,
			ITextualReader<TTextualFieldSpec, TTextualSpec>
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
				this.BaseTextReader.Dispose(); // no DisposeAsync on BTR class

			await Task.CompletedTask;
		}

		protected abstract IAsyncLifecycleEnumerable<TTextualFieldSpec> CoreReadHeaderFieldsAsync(CancellationToken cancellationToken = default);

		protected abstract IAsyncLifecycleEnumerable<ITextualStreamingRecord> CoreReadRecordsAsync(CancellationToken cancellationToken = default);

		public IAsyncLifecycleEnumerable<TTextualFieldSpec> ReadHeaderFieldsAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				return this.CoreReadHeaderFieldsAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual reader failed (see inner exception)."), ex);
			}
		}

		public IAsyncLifecycleEnumerable<ITextualStreamingRecord> ReadRecordsAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				return this.CoreReadRecordsAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual reader failed (see inner exception)."), ex);
			}
		}

		#endregion
	}
}
#endif