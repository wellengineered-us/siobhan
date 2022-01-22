/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
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

		protected abstract IAsyncLifecycleEnumerable<ITextualStreamingRecord> CoreReadFooterRecordsAsync(ILifecycleEnumerable<TTextualFieldSpec> footers, CancellationToken cancellationToken = default);

		protected abstract IAsyncLifecycleEnumerable<TTextualFieldSpec> CoreReadHeaderFieldsAsync(CancellationToken cancellationToken = default);

		protected abstract IAsyncLifecycleEnumerable<ITextualStreamingRecord> CoreReadRecordsAsync(CancellationToken cancellationToken = default);

		public IAsyncLifecycleEnumerable<ITextualStreamingRecord> ReadFooterRecordsAsync(ILifecycleEnumerable<TTextualFieldSpec> footers, CancellationToken cancellationToken = default)
		{
			if ((object)footers == null)
				throw new ArgumentNullException(nameof(footers));

			try
			{
				return this.CoreReadFooterRecordsAsync(footers, cancellationToken);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual reader failed (see inner exception)."), ex);
			}
		}

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