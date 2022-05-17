/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.Textual.Delimited
{
	public partial class DelimitedTextualWriter : TextualWriter<IDelimitedTextualFieldSpec, IDelimitedTextualSpec>
	{
		#region Methods/Operators

		protected override ValueTask CoreWriteFooterRecordsAsync(IAsyncLifecycleEnumerable<IDelimitedTextualFieldSpec> specs, IAsyncLifecycleEnumerable<ITextualStreamingRecord> footers, CancellationToken cancellationToken = default)
		{
			throw new NotSupportedException(string.Format("Cannot write footer records (via fields) in this version."));
		}

		protected override async ValueTask CoreWriteHeaderFieldsAsync(IAsyncLifecycleEnumerable<IDelimitedTextualFieldSpec> headers, CancellationToken cancellationToken = default)
		{
			if (this.HeaderRecordWritten)
				throw new InvalidOperationException(string.Format("Header record (fields) has (have) alredy been written."));

			// fields != null IF AND ONLY IF caller wishes to override DelimitedTextualSpec.DelimitedTextHeaderSpecs
			headers = headers ?? GetFieldsAsync(this.TextualSpec.TextualHeaderSpecs, cancellationToken).ToAsyncLifecycleEnumerable();

			if ((object)headers != null &&
				this.TextualSpec.IsFirstRecordHeader)
			{
				long fieldIndex = 0;
				await foreach (IDelimitedTextualFieldSpec header in headers)
				{
					await this.WriteFieldAsync(fieldIndex == 0, FormatFieldTitle(header.FieldTitle), cancellationToken);

					fieldIndex++;
				}

				if (!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter))
					await this.BaseTextWriter.WriteAsync(this.TextualSpec.RecordDelimiter);

				this.HeaderRecordWritten = true;
			}

			async IAsyncEnumerable<IDelimitedTextualFieldSpec> GetFieldsAsync(IList<IDelimitedTextualFieldSpec> textualSpecTextualHeaderSpecs, [EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				if ((object)textualSpecTextualHeaderSpecs == null)
					throw new ArgumentNullException(nameof(textualSpecTextualHeaderSpecs));

				// do not async here
				foreach (IDelimitedTextualFieldSpec textualSpecTextualHeaderSpec in this.TextualSpec.TextualHeaderSpecs)
				{
					yield return textualSpecTextualHeaderSpec;
				}

				await Task.CompletedTask;
			}
		}

		protected override async ValueTask CoreWriteRecordsAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> records, CancellationToken cancellationToken = default)
		{
			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			if (!this.HeaderRecordWritten)
				await this.WriteHeaderFieldsAsync(null, cancellationToken); // force fields if not explicitly called in advance

			long recordIndex = 0;
			await foreach (ISiobhanPayload record in records.WithCancellation(cancellationToken))
			{
				long fieldIndex = 0;
				foreach (KeyValuePair<string, object> item in record)
				{
					await this.WriteFieldAsync(fieldIndex == 0, this.FormatFieldValue(fieldIndex, item.Key, item.Value), cancellationToken);

					fieldIndex++;
				}

				if (!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter))
					await this.BaseTextWriter.WriteAsync(this.TextualSpec.RecordDelimiter);

				recordIndex++;
			}
		}

		private async Task WriteFieldAsync(bool firstFieldInRecord, string fieldValue, CancellationToken cancellationToken)
		{
			if (!firstFieldInRecord && !string.IsNullOrEmpty(this.TextualSpec.FieldDelimiter))
				await this.BaseTextWriter.WriteAsync(this.TextualSpec.FieldDelimiter);

			if (!string.IsNullOrEmpty(this.TextualSpec.OpenQuoteValue))
				await this.BaseTextWriter.WriteAsync(this.TextualSpec.OpenQuoteValue);

			await this.BaseTextWriter.WriteAsync(fieldValue);

			if (!string.IsNullOrEmpty(this.TextualSpec.CloseQuoteValue))
				await this.BaseTextWriter.WriteAsync(this.TextualSpec.CloseQuoteValue);
		}

		#endregion
	}
}
#endif