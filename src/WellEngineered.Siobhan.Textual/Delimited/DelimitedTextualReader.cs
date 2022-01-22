﻿/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
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
			this.ResetParserState();
		}

		#endregion

		#region Fields/Constants

		private readonly _ParserState parserState = new _ParserState();

		#endregion

		#region Properties/Indexers/Events

		private _ParserState ParserState
		{
			get
			{
				return this.parserState;
			}
		}

		#endregion

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

		protected override ILifecycleEnumerable<ITextualStreamingRecord> CoreReadFooterRecords(ILifecycleEnumerable<IDelimitedTextualFieldSpec> footers)
		{
			return GetFooters().ToLifecycleEnumerable();

			IEnumerable<ITextualStreamingRecord> GetFooters()
			{
				foreach (ITextualStreamingRecord textualStreamingRecord in new ITextualStreamingRecord[] { })
				{
					yield return textualStreamingRecord;
				}
			}
		}

		protected override ILifecycleEnumerable<IDelimitedTextualFieldSpec> CoreReadHeaderFields()
		{
			if (this.ParserState.recordIndex == 0 &&
				this.TextualSpec.IsFirstRecordHeader)
			{
				ITextualStreamingRecord header;
				IEnumerable<ITextualStreamingRecord> records = this.ResumableParserMainLoop(true);

				header = records.SingleOrDefault(); // force a single enumeration - yield return is a brain fyck

				// sanity check - should never non-null record since it breaks (once==true)
				if ((object)header != null)
					throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: yielded header record was not null."));

				this.FixupHeaderRecord();
			}

			return this.TextualSpec.TextualHeaderSpecs.ToLifecycleEnumerable();
		}

		protected override ILifecycleEnumerable<ITextualStreamingRecord> CoreReadRecords()
		{
			return this.ResumableParserMainLoop(false).ToLifecycleEnumerable();
		}

		private void FixupHeaderRecord()
		{
			string[] fieldNames;
			IDelimitedTextualFieldSpec delimitedTextualFieldSpec;

			fieldNames = this.ParserState.Header.Keys.ToArray();

			if ((object)this.TextualSpec == null ||
				(object)this.TextualSpec.TextualHeaderSpecs == null ||
				(object)fieldNames == null)
				throw new InvalidOperationException(string.Format(""));

			// stash parsed field names into field specs member
			if (fieldNames.Length == this.TextualSpec.TextualHeaderSpecs.Count)
			{
				for (int fieldIndex = 0; fieldIndex < fieldNames.Length; fieldIndex++)
				{
					delimitedTextualFieldSpec = this.TextualSpec.TextualHeaderSpecs[fieldIndex];

					if (!string.IsNullOrWhiteSpace(delimitedTextualFieldSpec.FieldTitle) &&
						delimitedTextualFieldSpec.FieldTitle.ToLower() != fieldNames[fieldIndex].ToLower())
						throw new InvalidOperationException(string.Format("Field name mismatch: '{0}' <> '{1}'.", delimitedTextualFieldSpec.FieldTitle, fieldNames[fieldIndex]));

					delimitedTextualFieldSpec.FieldTitle = fieldNames[fieldIndex];
				}
			}
			else
			{
				// reset field specs because they do not match in length
				this.TextualSpec.TextualHeaderSpecs.Clear();

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

					this.TextualSpec.TextualHeaderSpecs.Add(delimitedTextualFieldSpec);
				}
			}
		}

		private bool ParserStateMachine()
		{
			bool succeeded;
			string tempStringValue;
			bool matchedRecordDelimiter, matchedFieldDelimiter;

			// now determine what to do based on parser state
			matchedRecordDelimiter = !this.ParserState.isQuotedValue &&
									!string.IsNullOrEmpty(this.TextualSpec.RecordDelimiter) &&
									LookBehindFixup(this.ParserState.transientStringBuilder, this.TextualSpec.RecordDelimiter);

			if (!matchedRecordDelimiter)
			{
				matchedFieldDelimiter = !this.ParserState.isQuotedValue &&
										!string.IsNullOrEmpty(this.TextualSpec.FieldDelimiter) &&
										LookBehindFixup(this.ParserState.transientStringBuilder, this.TextualSpec.FieldDelimiter);
			}
			else
				matchedFieldDelimiter = false;

			if (matchedRecordDelimiter || matchedFieldDelimiter || this.ParserState.isEOF)
			{
				// RECORD_DELIMITER | FIELD_DELIMITER | EOF

				// get string and clear for exit
				tempStringValue = this.ParserState.transientStringBuilder.ToString();
				this.ParserState.transientStringBuilder.Clear();

				// common logic to store value of field in record
				if (this.ParserState.isHeaderRecord)
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

					delimitedTextualFieldSpecs = this.TextualSpec.TextualHeaderSpecs.ToArray();

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

			return false;
		}

		private void ResetParserState()
		{
			const long DEFAULT_INDEX = 0;

			this.ParserState.Record = new TextualStreamingRecord(0, 0, 0);
			this.ParserState.transientStringBuilder = new StringBuilder();
			this.ParserState.readCurrentCharacter = '\0';
			this.ParserState.peekNextCharacter = '\0';
			this.ParserState.characterIndex = DEFAULT_INDEX;
			this.ParserState.contentIndex = DEFAULT_INDEX;
			this.ParserState.recordIndex = DEFAULT_INDEX;
			this.ParserState.fieldIndex = DEFAULT_INDEX;
			this.ParserState.valueIndex = DEFAULT_INDEX;
			this.ParserState.isQuotedValue = false;
			this.ParserState.isEOF = false;

			this.TextualSpec.AssertValid();
		}

		private IEnumerable<ITextualStreamingRecord> ResumableParserMainLoop(bool once)
		{
			int __value;
			char ch;

			// main loop - character stream
			while (!this.ParserState.isEOF)
			{
				// read the next byte
				__value = this.BaseTextReader.Read();
				ch = (char)__value;

				// check for -1 (EOF)
				if (__value == -1)
				{
					this.ParserState.isEOF = true; // set terminal state

					// sanity check - should never end with an open quote value
					if (this.ParserState.isQuotedValue)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: end of file encountered while reading open quoted value."));
				}
				else
				{
					// append character to temp buffer
					this.ParserState.readCurrentCharacter = ch;
					this.ParserState.transientStringBuilder.Append(ch);

					// advance character index
					this.ParserState.characterIndex++;
				}

				// eval on every loop
				this.ParserState.isHeaderRecord = this.ParserState.recordIndex == 0 && this.TextualSpec.IsFirstRecordHeader;
				this.ParserState.isFooterRecord = false; //this.ParserState.recordIndex == 0 && (this.TextualSpec.IsLastRecordFooter ?? false);

				// peek the next byte
				__value = this.BaseTextReader.Peek();
				ch = (char)__value;
				this.ParserState.peekNextCharacter = ch;

				if (this.ParserStateMachine())
				{
					// if record is null here, then is was a blank line - no error just avoid doing work
					if ((object)this.ParserState.Record != null)
					{
						// should never yield the header record
						if (!this.ParserState.isHeaderRecord)
						{
							// aint this some shhhhhhhh!t?
							yield return this.ParserState.Record;
						}
						else
						{
							this.ParserState.Header = this.ParserState.Record; // cache elsewhere
							this.ParserState.Record = null; // pretend it was a blank line
							//this.ParserState.recordIndex--; // adjust down to zero
						}
					}

					// sanity check - should never get here with zero record index
					if ( /*!this.ParserState.isHeaderRecord &&*/ this.ParserState.recordIndex == 0)
						throw new InvalidOperationException(string.Format("Delimited text reader parse state failure: zero record index unexpected."));

					// create a new record for the next index; will be used later
					this.ParserState.Record = new TextualStreamingRecord(this.ParserState.recordIndex, this.ParserState.contentIndex, this.ParserState.characterIndex);

					if (once) // state-based resumption of loop ;)
						break; // MUST NOT USE YIELD BREAK - as we will RESUME the enumeration based on state
				}
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		private sealed class _ParserState
		{
			#region Fields/Constants

			public long characterIndex;
			public long contentIndex;
			public long fieldIndex;
			private ITextualStreamingRecord header;
			public bool isEOF;
			public bool isFooterRecord;
			public bool isHeaderRecord;
			public bool isQuotedValue;
			public char peekNextCharacter;
			public char readCurrentCharacter;
			private ITextualStreamingRecord record;
			public long recordIndex;
			public StringBuilder transientStringBuilder;
			public long valueIndex;

			#endregion

			#region Properties/Indexers/Events

			public ITextualStreamingRecord Header
			{
				get
				{
					return this.header;
				}
				set
				{
					this.header = value;
				}
			}

			public ITextualStreamingRecord Record
			{
				get
				{
					return this.record;
				}
				set
				{
					this.record = value;
				}
			}

			#endregion
		}

		#endregion
	}
}