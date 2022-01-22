/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;

using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.Textual.Delimited
{
	public partial class DelimitedTextualWriter : TextualWriter<IDelimitedTextualFieldSpec, IDelimitedTextualSpec>
	{
		#region Constructors/Destructors

		public DelimitedTextualWriter(TextWriter baseTextWriter, IDelimitedTextualSpec delimitedTextualSpec)
			: base(baseTextWriter, delimitedTextualSpec)
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

		protected static string FormatFieldTitle(string fieldTitle)
		{
			string value;

			if ((object)fieldTitle == null)
				throw new ArgumentNullException(nameof(fieldTitle));

			value = fieldTitle; // TODO: escape bad chars

			return value;
		}

		protected override void CoreWriteFooterRecords(ILifecycleEnumerable<IDelimitedTextualFieldSpec> footers, ILifecycleEnumerable<ITextualStreamingRecord> records)
		{
			throw new NotSupportedException(string.Format("Cannot write footer records (via fields) in this version."));
		}

		protected override void CoreWriteHeaderFields(ILifecycleEnumerable<IDelimitedTextualFieldSpec> headers)
		{
			if (this.HeaderRecordWritten)
				throw new InvalidOperationException(string.Format("Header record (fields) has (have) already been written."));

			// fields != null IF AND ONLY IF caller wishes to override DelimitedTextualSpec.DelimitedTextHeaderSpecs
			headers = headers ?? this.TextualSpec.TextualHeaderSpecs.ToLifecycleEnumerable();

			if ((object)headers != null &&
				this.TextualSpec.IsFirstRecordHeader)
			{
				long fieldIndex = 0;
				foreach (IDelimitedTextualFieldSpec header in headers)
				{
					this.WriteField(fieldIndex == 0, FormatFieldTitle(header.FieldTitle));

					fieldIndex++;
				}

				if (!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter))
					this.BaseTextWriter.Write(this.TextualSpec.RecordDelimiter);

				this.HeaderRecordWritten = true;
			}
		}

		protected override void CoreWriteRecords(ILifecycleEnumerable<ISiobhanPayload> records)
		{
			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			if (!this.HeaderRecordWritten)
				this.WriteHeaderFields(null); // force fields if not explicitly called in advance

			long recordIndex = 0;
			foreach (ISiobhanPayload record in records)
			{
				long fieldIndex = 0;
				foreach (KeyValuePair<string, object> item in record)
				{
					this.WriteField(fieldIndex == 0, this.FormatFieldValue(fieldIndex, item.Key, item.Value));

					fieldIndex++;
				}

				if (!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter))
					this.BaseTextWriter.Write(this.TextualSpec.RecordDelimiter);

				recordIndex++;
			}
		}

		protected string FormatFieldValue(long fieldIndex, string fieldTitle, object fieldValue)
		{
			IDelimitedTextualFieldSpec header = null;
			string value;
			string safeFieldValue;

			if ((object)fieldTitle == null)
				throw new ArgumentNullException(nameof(fieldTitle));

			// TODO: do not assume order is corrcetly aligned to index
			if (fieldIndex < this.TextualSpec.TextualHeaderSpecs.Count)
				header = this.TextualSpec.TextualHeaderSpecs[(int)fieldIndex];

			safeFieldValue = fieldValue?.ToString() ?? string.Empty;

			if ((object)header != null && !string.IsNullOrEmpty(header.FieldFormat))
				value = string.Format("{0:" + header.FieldFormat + "}", safeFieldValue);
			else
				value = safeFieldValue;

			return value;
		}

		private void WriteField(bool firstFieldInRecord, string fieldValue)
		{
			if (!firstFieldInRecord && !string.IsNullOrEmpty(this.TextualSpec.FieldDelimiter))
				this.BaseTextWriter.Write(this.TextualSpec.FieldDelimiter);

			if (!string.IsNullOrEmpty(this.TextualSpec.OpenQuoteValue))
				this.BaseTextWriter.Write(this.TextualSpec.OpenQuoteValue);

			this.BaseTextWriter.Write(fieldValue);

			if (!string.IsNullOrEmpty(this.TextualSpec.CloseQuoteValue))
				this.BaseTextWriter.Write(this.TextualSpec.CloseQuoteValue);
		}

		#endregion
	}
}