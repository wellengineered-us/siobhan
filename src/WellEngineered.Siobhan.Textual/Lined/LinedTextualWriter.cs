/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
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

		#region Methods/Operators

		protected override void CoreWriteHeaderFields(ILifecycleEnumerable<ILinedTextualFieldSpec> headers)
		{
			// do nothing
		}

		protected override void CoreWriteRecords(ILifecycleEnumerable<ISiobhanPayload> records)
		{
			if ((object)records == null)
				throw new ArgumentNullException(nameof(records));

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

				string newline;
				switch (this.TextualSpec.NewLineStyle)
				{
					case NewLineStyle.Auto:
						newline = Environment.NewLine;
						break;
					case NewLineStyle.Windows:
						newline = "\r\n";
						break;
					case NewLineStyle.Unix:
						newline = "\n";
						break;
					default:
						throw new ArgumentOutOfRangeException(this.TextualSpec.NewLineStyle.ToString());
				}
				
				this.BaseTextWriter.Write(newline);

				recordIndex++;
			}
		}

		#endregion
	}
}