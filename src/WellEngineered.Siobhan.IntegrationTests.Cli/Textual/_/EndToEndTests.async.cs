/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using WellEngineered.Siobhan.Textual.Delimited;
using WellEngineered.Siobhan.Textual.Lined;

namespace WellEngineered.Siobhan.IntegrationTests.Cli.Textual.@_
{
	[TestFixture]
	public partial class EndToEndTests
	{
		#region Methods/Operators

		[Test]
		public async ValueTask ShouldParseSampleLinedFileTestAsync()
		{
			string textualFilePath = @"d:\SupplierData.csv";
			LinedTextualSpec delimitedTextualSpec = new LinedTextualSpec()
													{
														ContentEncoding = "us-ascii"
													};
			
			using (StreamReader streamReader = new StreamReader(File.Open(textualFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.GetEncoding(delimitedTextualSpec.ContentEncoding)))
			{
				await using (LinedTextualReader textualReader = new LinedTextualReader(streamReader, delimitedTextualSpec))
				{
					await foreach (var header in textualReader.ReadHeaderFieldsAsync())
					{
						await Console.Out.WriteLineAsync(string.Format("{0:0000}: {1}", header.FieldOrdinal, header.FieldTitle));
					}

					await foreach (var record in textualReader.ReadRecordsAsync())
					{
						await Console.Out.WriteLineAsync(string.Format("cns={0:000000000}, cne={1:000000000}, ln={2:000000000}, ri={3:000000000}", record.CharacterNumberStart, record.CharacterNumberEnd, record.LineNumber, record.RecordIndex));
					}
				}
			}
		}
		
		[Test]
		public async ValueTask ShouldParseSampleDelimitedFileTestAsync()
		{
			string textualFilePath = @"d:\SupplierData.csv";
			DelimitedTextualSpec delimitedTextualSpec = new DelimitedTextualSpec()
													{
														ContentEncoding = "us-ascii",
														IsFirstRecordHeader = true,
														RecordDelimiter = "\r\n"
													};
			
			using (StreamReader streamReader = new StreamReader(File.Open(textualFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.GetEncoding(delimitedTextualSpec.ContentEncoding)))
			{
				await using (DelimitedTextualReader textualReader = new DelimitedTextualReader(streamReader, delimitedTextualSpec))
				{
					await foreach (var header in textualReader.ReadHeaderFieldsAsync())
					{
						await Console.Out.WriteLineAsync(string.Format("{0:0000}: {1}", header.FieldOrdinal, header.FieldTitle));
					}

					await foreach (var record in textualReader.ReadRecordsAsync())
					{
						await Console.Out.WriteLineAsync(string.Format("cns={0:000000000}, cne={1:000000000}, ln={2:000000000}, ri={3:000000000}", record.CharacterNumberStart, record.CharacterNumberEnd, record.LineNumber, record.RecordIndex));
					}
				}
			}
		}

		#endregion
	}
}
#endif