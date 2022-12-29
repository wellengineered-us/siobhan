/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using WellEngineered.Siobhan.Textual.Delimited;
using WellEngineered.Siobhan.Textual.Lined;

namespace WellEngineered.Siobhan.IntegrationTests.Cli.Textual.@_
{
	[TestFixture]
	public partial class EndToEndTests
	{
		#region Constructors/Destructors

		public EndToEndTests()
		{
		}

		#endregion

		#region Methods/Operators
		
		[Test]
		public void ShouldParseSampleLinedFileAsciiEncodingUnixEolTest()
		{
			ParseSampleLinedFileTest(@"d:\testdata.small.ascii.unix.csv", "us-ascii", NewLineStyle.Unix);
		}
		
		[Test]
		public void ShouldParseSampleLinedFileAsciiEncodingWindowsEolTest()
		{
			ParseSampleLinedFileTest(@"d:\testdata.small.ascii.windows.csv", "us-ascii", NewLineStyle.Windows);
		}
		
		private void ParseSampleLinedFileTest(string textualFilePath, string contentEncoding, NewLineStyle newLineStyle)
		{
			LinedTextualSpec delimitedTextualSpec = new LinedTextualSpec()
													{
														ContentEncoding = contentEncoding,
														NewLineStyle = newLineStyle
													};
			
			using (StreamReader streamReader = new StreamReader(File.Open(textualFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.GetEncoding(delimitedTextualSpec.ContentEncoding)))
			{
				using (LinedTextualReader textualReader = new LinedTextualReader(streamReader, delimitedTextualSpec))
				{
					foreach (var header in textualReader.ReadHeaderFields())
					{
						Console.WriteLine("{0:0000}: {1}", header.FieldOrdinal, header.FieldTitle);
					}

					foreach (var record in textualReader.ReadRecords())
					{
						Console.WriteLine("cns={0:000000000}, cne={1:000000000}, ln={2:000000000}, ri={3:000000000}\t{4}", record.CharacterNumberStart, record.CharacterNumberEnd, record.LineNumber, record.RecordIndex, record[string.Empty]);
					}
				}
			}
		}

		[Test]
		public void ShouldParseSampleDelimitedFileTest()
		{
			string textualFilePath = @"d:\SupplierData.csv";
			DelimitedTextualSpec delimitedTextualSpec = new DelimitedTextualSpec()
														{
															ContentEncoding = "us-ascii",
															IsFirstRecordHeader = true,
															RecordDelimiter = "\r\n",
															FieldDelimiter = ",",
															CloseQuoteValue = "\"",
															OpenQuoteValue = "\""
														};
			
			using (StreamReader streamReader = new StreamReader(File.Open(textualFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), Encoding.GetEncoding(delimitedTextualSpec.ContentEncoding)))
			{
				using (DelimitedTextualReader textualReader = new DelimitedTextualReader(streamReader, delimitedTextualSpec))
				{
					foreach (var header in textualReader.ReadHeaderFields())
					{
						Console.WriteLine("{0:0000}: {1}", header.FieldOrdinal, header.FieldTitle);
					}

					foreach (var record in textualReader.ReadRecords())
					{
						Console.WriteLine("cns={0:000000000}, cne={1:000000000}, ln={2:000000000}, ri={3:000000000}\t{4}", record.CharacterNumberStart, record.CharacterNumberEnd, record.LineNumber, record.RecordIndex, record[string.Empty]);
					}
				}
			}
		}

		#endregion
	}
}