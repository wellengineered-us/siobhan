/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.Textual.Lined
{
	public partial class LinedTextualWriter
		: TextualWriter<ILinedTextualFieldSpec,
			ILinedTextualSpec>
	{
		#region Constructors/Destructors

		public LinedTextualWriter(TextWriter baseTextWriter, ILinedTextualSpec linedTextualSpec)
			: base(baseTextWriter, linedTextualSpec)
		{
		}

		#endregion

		#region Fields/Constants

		private bool footerRecordWritten;
		private bool headerRecordWritten;

		#endregion

		#region Properties/Indexers/Events

		private bool FooterRecordWritten
		{
			get
			{
				return this.footerRecordWritten;
			}
			set
			{
				this.footerRecordWritten = value;
			}
		}

		private bool HeaderRecordWritten
		{
			get
			{
				return this.headerRecordWritten;
			}
			set
			{
				this.headerRecordWritten = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreWriteFooterRecords(ILifecycleEnumerable<ILinedTextualFieldSpec> footers, ILifecycleEnumerable<ITextualStreamingRecord> records)
		{
			// do nothing
		}

		protected override void CoreWriteHeaderFields(ILifecycleEnumerable<ILinedTextualFieldSpec> headers)
		{
			// do nothing
		}

		protected override void CoreWriteRecords(ILifecycleEnumerable<ISiobhanPayload> records)
		{
			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			if (!this.HeaderRecordWritten)
				this.CoreWriteHeaderFields(null); // force fields if not explicitly called in advance

			long recordIndex = 0;
			foreach (ISiobhanPayload record in records)
			{
				long fieldIndex = 0;
				if (record.TryGetValue(string.Empty, out object rawFieldValue))
				{
					string safeFieldValue;

					safeFieldValue = rawFieldValue == null ? string.Empty : rawFieldValue.ToString();

					this.BaseTextWriter.Write(safeFieldValue);

					fieldIndex++;
				}

				if (!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter))
					this.BaseTextWriter.Write(this.TextualSpec.RecordDelimiter);

				recordIndex++;
			}
		}

		#endregion
	}
}