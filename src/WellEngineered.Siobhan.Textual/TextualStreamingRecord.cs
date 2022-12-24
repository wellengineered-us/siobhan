/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Textual
{
	public sealed class TextualStreamingRecord : Dictionary<string, object>, ITextualStreamingRecord
	{
		#region Constructors/Destructors

		public TextualStreamingRecord(long recordIndex, long lineNumber, long characterNumberStart, long characterNumberEnd)
			: base(StringComparer.OrdinalIgnoreCase)
		{
			this.recordIndex = recordIndex;
			this.lineNumber = lineNumber;
			this.characterNumberStart = characterNumberStart;
			this.characterNumberEnd = characterNumberEnd;
		}

		#endregion

		#region Fields/Constants

		private readonly long characterNumberStart;
		private readonly long characterNumberEnd;
		private readonly long lineNumber;
		private readonly long recordIndex;

		#endregion

		#region Properties/Indexers/Events

		public long CharacterNumberStart
		{
			get
			{
				return this.characterNumberStart;
			}
		}
		
		public long CharacterNumberEnd
		{
			get
			{
				return this.characterNumberEnd;
			}
		}

		public long LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public long RecordIndex
		{
			get
			{
				return this.recordIndex;
			}
		}

		#endregion
	}
}