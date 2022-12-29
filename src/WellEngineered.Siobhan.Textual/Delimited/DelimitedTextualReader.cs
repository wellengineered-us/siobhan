/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual.Delimited
{
	public partial class DelimitedTextualReader
		: TextualReader<IDelimitedTextualFieldSpec, IDelimitedTextualSpec>
	{
		#region Constructors/Destructors

		public DelimitedTextualReader(TextReader baseTextReader, IDelimitedTextualSpec delimitedTextualSpec)
			: base(baseTextReader, delimitedTextualSpec)
		{
		}

		#endregion

		private bool isFirstRecord;

		public bool IsFirstRecord
		{
			get
			{
				return this.isFirstRecord;
			}
			set
			{
				this.isFirstRecord = value;
			}
		}

		#region Methods/Operators
		
		private static bool LookBehindFixup(StringBuilder targetStringBuilder, string targetValue)
		{
			if ((object)targetStringBuilder == null)
				throw new ArgumentNullException(nameof(targetStringBuilder));

			if ((object)targetValue == null)
				throw new ArgumentNullException(nameof(targetValue));

			if (string.IsNullOrEmpty(targetValue))
				throw new ArgumentOutOfRangeException(nameof(targetValue));

			// look-behind CHECK
			if (targetStringBuilder.Length > 0 &&
				targetValue.Length > 0 &&
				targetStringBuilder.Length >= targetValue.Length)
			{
				int sb_length;
				int rd_length;
				int matches;

				sb_length = targetStringBuilder.Length;
				rd_length = targetValue.Length;
				matches = 0;

				for (int i = 0; i < rd_length; i++)
				{
					if (targetStringBuilder[sb_length - rd_length + i] != targetValue[i])
						return false; // look-behind NO MATCH...

					matches++;
				}

				if (matches != rd_length)
					throw new InvalidOperationException(string.Format("Something went sideways."));

				targetStringBuilder.Remove(sb_length - rd_length, rd_length);

				// look-behind MATCHED: stop
				return true;
			}

			return false; // not enough buffer space to care
		}
		
		private static void FixupHeaderRecord(IDelimitedTextualSpec delimitedTextualSpec, ITextualStreamingRecord header)
		{
			string[] fieldNames;
			IDelimitedTextualFieldSpec delimitedTextualFieldSpec;

			if ((object)delimitedTextualSpec == null)
				throw new ArgumentNullException(nameof(delimitedTextualSpec));

			if ((object)header == null)
				throw new ArgumentNullException(nameof(header));
			
			fieldNames = header.Keys.ToArray();
			
			// stash parsed field names into field specs member
			if (fieldNames.Length == delimitedTextualSpec.HeaderSpecs.Count)
			{
				for (int fieldIndex = 0; fieldIndex < fieldNames.Length; fieldIndex++)
				{
					delimitedTextualFieldSpec = delimitedTextualSpec.HeaderSpecs[fieldIndex];

					if (!string.IsNullOrWhiteSpace(delimitedTextualFieldSpec.FieldTitle) &&
						delimitedTextualFieldSpec.FieldTitle.ToLower() != fieldNames[fieldIndex].ToLower())
						throw new InvalidOperationException(string.Format("Field name mismatch: '{0}' <> '{1}'.", delimitedTextualFieldSpec.FieldTitle, fieldNames[fieldIndex]));

					delimitedTextualFieldSpec.FieldTitle = fieldNames[fieldIndex];
				}
			}
			else
			{
				// reset field specs because they do not match in length
				delimitedTextualSpec.HeaderSpecs.Clear();

				for (long fieldIndex = 0; fieldIndex < fieldNames.Length; fieldIndex++)
				{
					delimitedTextualFieldSpec = new DelimitedTextualFieldSpec()
												{
													FieldTitle = fieldNames[fieldIndex],
													FieldType = TextualFieldType.Text,
													IsFieldIdentity = false,
													IsFieldRequired = true,
													FieldFormat = null,
													FieldOrdinal = fieldIndex
												};

					delimitedTextualSpec.HeaderSpecs.Add(delimitedTextualFieldSpec);
				}
			}
		}

		protected override ILifecycleEnumerable<IDelimitedTextualFieldSpec> CoreReadHeaderFields()
		{
			if (this.IsFirstRecord &&
				this.TextualSpec.IsFirstRecordHeader)
			{
				ITextualStreamingRecord header;
				IEnumerable<ITextualStreamingRecord> records = this.ResumableParserMainLoop(true);

				header = records.SingleOrDefault(); // force a single enumeration - yield return is a brain fyck

				// sanity check - should never non-null record since it breaks (yieldOnlyOnce==true)
				if ((object)header != null)
					throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: yielded header record was not null."));

				FixupHeaderRecord(this.TextualSpec, header);
			}

			return this.TextualSpec.HeaderSpecs.ToLifecycleEnumerable();
		}

		protected override ILifecycleEnumerable<ITextualStreamingRecord> CoreReadRecords()
		{
			return this.ResumableParserMainLoop(false).ToLifecycleEnumerable();
		}

		private IEnumerable<ITextualStreamingRecord> ResumableParserMainLoop(bool yieldOnlyOnce)
		{
			throw new NotImplementedException();
			/*const int EOF = -1;
			const int DEFAULT_INDEX = -1;
			string line;
			
			ITextualStreamingRecord record;
			bool isEndOfFile = false;
			long recordIndex = DEFAULT_INDEX;
			long lineIndex = DEFAULT_INDEX;
			long characterIndexStart = DEFAULT_INDEX;
			long characterIndexEnd = DEFAULT_INDEX;

			StringBuilder parserStringBuilder;

			char previous = '\0';
			
			// (open_quote) [value] (close_quote) [field_delim] ... [record_delim] | [eof]

			bool isHeaderRecord;
			bool isQuotedValue = false;
			bool matchedRecordDelimiter;
			bool matchedFieldDelimiter;
			
			this.TextualSpec.AssertValid();

			parserStringBuilder = new StringBuilder();
			
			// main loop - character stream
			while (!isEndOfFile)
			{
				int __value;
				char current, next;
				
				// read the next byte
				__value = this.BaseTextReader.Read();
				current = (char)__value;
				
				// check for -1 (EOF)
				if (__value == EOF)
				{
					// set terminal state
					isEndOfFile = true;
					
					// sanity check - should never end with an open quote value
					if (isQuotedValue)
						throw new SiobhanException(string.Format("Delimited text reader parse state failure: end of file encountered while reading open quoted value."));

					next = '\0';
				}
				else
				{
					// peek the next byte
					__value = this.BaseTextReader.Peek();
					next = (char)__value;
					
					// append character to temp buffer
					parserStringBuilder.Append(current);
				}

				// eval on every loop
				isHeaderRecord = recordIndex == 0 && this.TextualSpec.IsFirstRecordHeader;
				
				// check parser state
				matchedRecordDelimiter = !isQuotedValue &&
										!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter) &&
										LookBehindFixup(parserStringBuilder, this.TextualSpec.RecordDelimiter);

				if (!matchedRecordDelimiter)
				{
					matchedFieldDelimiter = !isQuotedValue &&
											!string.IsNullOrEmpty(this.TextualSpec.FieldDelimiter) &&
											LookBehindFixup(parserStringBuilder, this.TextualSpec.FieldDelimiter);
				}
				else
					matchedFieldDelimiter = false;
				
				if (matchedRecordDelimiter || matchedFieldDelimiter || isEndOfFile)
				{
					// RECORD_DELIMITER | FIELD_DELIMITER | EOF

					// get string and clear for exit
					tempStringValue = this.ParserState.transientStringBuilder.ToString();
					this.ParserState.transientStringBuilder.Clear();

					// common logic to store value of field in record
					if (isHeaderRecord)
					{
						// stash field if FRIS enabled and zeroth record
						this.ParserState.Record.Add(tempStringValue, this.ParserState.fieldIndex.ToString("0000"));
					}
					else
					{
						Type fieldType;
						TextualFieldType textualFieldType;
						object fieldValue;

						IDelimitedTextualFieldSpec delimitedTextualFieldSpec;
						IDelimitedTextualFieldSpec[] delimitedTextualFieldSpecs;

						delimitedTextualFieldSpecs = this.TextualSpec.HeaderSpecs.ToArray();

						// check field array and field index validity
						if ((object)delimitedTextualFieldSpecs == null ||
							this.ParserState.fieldIndex >= delimitedTextualFieldSpecs.LongLength)
							throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: field index '{0}' exceeded known field indices '{1}' at character index '{2}'.", this.ParserState.fieldIndex, (object)delimitedTextualFieldSpecs != null ? (delimitedTextualFieldSpecs.Length - 1) : (int?)null, this.ParserState.characterIndex));

						delimitedTextualFieldSpec = delimitedTextualFieldSpecs[this.ParserState.fieldIndex];

						textualFieldType = delimitedTextualFieldSpec.FieldType;

						fieldType = textualFieldType.ToClrType();

						// TODO: add default values, null field value conversions
						succeeded = true;
						if (string.IsNullOrWhiteSpace(tempStringValue))
							fieldValue = fieldType.DefaultValue();
						else
							succeeded = tempStringValue.TryParse(fieldType, out fieldValue);

						if (!succeeded)
							throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: field string value '{0}' could not be parsed into a valid '{1}'.", tempStringValue, fieldType.FullName));

						// lookup field name (key) by index and commit value to record
						this.ParserState.Record.Add(delimitedTextualFieldSpec.FieldTitle, fieldValue);
					}

					// handle blank lines (we assume that any RECORDS with valid RECORD delimiter is OK)
					if (string.IsNullOrEmpty(tempStringValue) &&
						this.ParserState.Record.Keys.Count == 1)
						this.ParserState.Record = null;

					// now what to do?
					if (this.ParserState.isEOF)
						return true;
					else if (matchedRecordDelimiter)
					{
						// advance record index
						this.ParserState.recordIndex++;

						// reset field index
						this.ParserState.fieldIndex = 0;

						// reset value index
						this.ParserState.valueIndex = 0;

						return true;
					}
					else if (matchedFieldDelimiter)
					{
						// advance field index
						this.ParserState.fieldIndex++;

						// reset value index
						this.ParserState.valueIndex = 0;
					}
				}
				else if (!this.ParserState.isEOF &&
						!this.ParserState.isQuotedValue &&
						!string.IsNullOrEmpty(this.TextualSpec.OpenQuoteValue) &&
						LookBehindFixup(this.ParserState.transientStringBuilder, this.TextualSpec.OpenQuoteValue))
				{
					// BEGIN::QUOTE_VALUE
					this.ParserState.isQuotedValue = true;
				}
				//else if (!this.ParserState.isEOF &&
				//	this.ParserState.isQuotedValue &&
				//	!string.IsNullOrEmpty(this.TextualSpec.QuoteValue) &&
				//	LookBehindFixup(this.ParserState.transientStringBuilder, this.TextualSpec.QuoteValue) &&
				//	this.ParserState.peekNextCharacter.ToString() == this.TextualSpec.QuoteValue)
				//{
				//	// unescape::QUOTE_VALUE
				//	this.ParserState.transientStringBuilder.Append("'");
				//}
				else if (!this.ParserState.isEOF &&
						this.ParserState.isQuotedValue &&
						!string.IsNullOrEmpty(this.TextualSpec.CloseQuoteValue) &&
						LookBehindFixup(this.ParserState.transientStringBuilder, this.TextualSpec.CloseQuoteValue))
				{
					// END::QUOTE_VALUE
					this.ParserState.isQuotedValue = false;
				}
				else if (!this.ParserState.isEOF)
				{
					// {field_data}

					// advance content index
					this.ParserState.contentIndex++;

					// advance value index
					this.ParserState.valueIndex++;
				}
				else
				{
					// {unknown_parser_state_error}
					throw new InvalidOperationException(string.Format("Unknown parser state error at character index '{0}'.", this.ParserState.characterIndex));
				}
				
				previous = current;
			}*/
		}

		#endregion
	}
}