/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public abstract partial class TextualReader<TTextualFieldSpec, TTextualSpec>
#if ASYNC_ALL_THE_WAY_DOWN
		: DualLifecycle,
#else
		: Lifecycle,
#endif
			ITextualReader<TTextualFieldSpec, TTextualSpec>
		where TTextualFieldSpec : ITextualFieldSpec
		where TTextualSpec : ITextualSpec<TTextualFieldSpec>
	{
		#region Constructors/Destructors

		protected TextualReader(TextReader baseTextReader, TTextualSpec textualSpec)
		{
			if ((object)baseTextReader == null)
				throw new ArgumentNullException(nameof(baseTextReader));

			if ((object)textualSpec == null)
				throw new ArgumentNullException(nameof(textualSpec));

			this.baseTextReader = baseTextReader;
			this.textualSpec = textualSpec;
		}

		#endregion

		#region Fields/Constants

		private readonly TextReader baseTextReader;
		private readonly TTextualSpec textualSpec;

		#endregion

		#region Properties/Indexers/Events

		public TextReader BaseTextReader
		{
			get
			{
				return this.baseTextReader;
			}
		}

		public TTextualSpec TextualSpec
		{
			get
			{
				return this.textualSpec;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreCreate(bool creating)
		{
			// do nothing
		}

		protected override void CoreDispose(bool disposing)
		{
			if (disposing)
				this.BaseTextReader.Dispose();
		}

		protected abstract ILifecycleEnumerable<ITextualStreamingRecord> CoreReadFooterRecords(ILifecycleEnumerable<TTextualFieldSpec> footers);

		protected abstract ILifecycleEnumerable<TTextualFieldSpec> CoreReadHeaderFields();

		protected abstract ILifecycleEnumerable<ITextualStreamingRecord> CoreReadRecords();

		public ILifecycleEnumerable<ITextualStreamingRecord> ReadFooterRecords(ILifecycleEnumerable<TTextualFieldSpec> footers)
		{
			if ((object)footers == null)
				throw new ArgumentNullException(nameof(footers));

			try
			{
				return this.CoreReadFooterRecords(footers);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual reader failed (see inner exception)."), ex);
			}
		}

		public ILifecycleEnumerable<TTextualFieldSpec> ReadHeaderFields()
		{
			try
			{
				return this.CoreReadHeaderFields();
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual reader failed (see inner exception)."), ex);
			}
		}

		public ILifecycleEnumerable<ITextualStreamingRecord> ReadRecords()
		{
			try
			{
				return this.CoreReadRecords();
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual reader failed (see inner exception)."), ex);
			}
		}

		#endregion
	}
}