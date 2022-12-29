/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public abstract partial class TextualWriter<TTextualFieldSpec, TTextualSpec>
#if ASYNC_ALL_THE_WAY_DOWN
		: DualLifecycle,
#else
			: Lifecycle,
#endif
			ITextualWriter<TTextualFieldSpec, TTextualSpec>
		where TTextualFieldSpec : ITextualFieldSpec
		where TTextualSpec : ITextualSpec<TTextualFieldSpec>
	{
		#region Constructors/Destructors

		protected TextualWriter(TextWriter baseTextWriter, TTextualSpec textualSpec)
		{
			if ((object)baseTextWriter == null)
				throw new ArgumentNullException(nameof(baseTextWriter));

			if ((object)textualSpec == null)
				throw new ArgumentNullException(nameof(textualSpec));

			this.baseTextWriter = baseTextWriter;
			this.textualSpec = textualSpec;
		}

		#endregion

		#region Fields/Constants

		private readonly TextWriter baseTextWriter;
		private readonly TTextualSpec textualSpec;

		#endregion

		#region Properties/Indexers/Events

		public TextWriter BaseTextWriter
		{
			get
			{
				return this.baseTextWriter;
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
				this.BaseTextWriter.Dispose();
		}

		protected virtual void CoreFlush()
		{
			this.BaseTextWriter.Flush();
		}

		protected abstract void CoreWriteHeaderFields(ILifecycleEnumerable<TTextualFieldSpec> headers);

		protected abstract void CoreWriteRecords(ILifecycleEnumerable<ISiobhanPayload> records);

		public void Flush()
		{
			try
			{
				this.CoreFlush();
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual writer failed (see inner exception)."), ex);
			}
		}

		public void WriteHeaderFields(ILifecycleEnumerable<TTextualFieldSpec> headers)
		{
			//if ((object)headers == null)
				//throw new ArgumentNullException(nameof(headers));

			try
			{
				this.CoreWriteHeaderFields(headers);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual writer failed (see inner exception)."), ex);
			}
		}

		public void WriteRecords(ILifecycleEnumerable<ISiobhanPayload> records)
		{
			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			try
			{
				this.CoreWriteRecords(records);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The textual writer failed (see inner exception)."), ex);
			}
		}

		#endregion
	}
}