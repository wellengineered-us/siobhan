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

namespace WellEngineered.Siobhan.Textual.Lined
{
	public partial class LinedTextualWriter
		: TextualWriter<ILinedTextualFieldSpec,
			ILinedTextualSpec>
	{
		#region Methods/Operators

		protected override ValueTask CoreWriteFooterRecordsAsync(IAsyncLifecycleEnumerable<ILinedTextualFieldSpec> specs, IAsyncLifecycleEnumerable<ITextualStreamingRecord> footers, CancellationToken cancellationToken = default)
		{
			// do nothing
			return default;
		}

		protected override ValueTask CoreWriteHeaderFieldsAsync(IAsyncLifecycleEnumerable<ILinedTextualFieldSpec> headers, CancellationToken cancellationToken = default)
		{
			// do nothing
			return default;
		}

		protected override async ValueTask CoreWriteRecordsAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> records, CancellationToken cancellationToken = default)
		{
			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

			if (!this.HeaderRecordWritten)
				await this.CoreWriteHeaderFieldsAsync(null, cancellationToken); // force fields if not explicitly called in advance

			long recordIndex = 0;
			await foreach (ISiobhanPayload record in records.WithCancellation(cancellationToken))
			{
				long fieldIndex = 0;
				if (record.TryGetValue(string.Empty, out object rawFieldValue))
				{
					string safeFieldValue;

					safeFieldValue = rawFieldValue == null ? string.Empty : rawFieldValue.ToString();

					await this.BaseTextWriter.WriteAsync(safeFieldValue);

					fieldIndex++;
				}

				if (!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter))
					await this.BaseTextWriter.WriteAsync(this.TextualSpec.RecordDelimiter);

				recordIndex++;
			}
		}

		#endregion
	}
}
#endif