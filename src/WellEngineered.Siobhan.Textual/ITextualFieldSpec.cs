/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Textual
{
	public interface ITextualFieldSpec
	{
		#region Properties/Indexers/Events

		string FieldFormat
		{
			get;
		}

		long FieldOrdinal
		{
			get;
		}

		TextualFieldType FieldType
		{
			get;
		}

		bool IsFieldIdentity
		{
			get;
		}

		bool IsFieldRequired
		{
			get;
		}

		string FieldTitle
		{
			get;
			set;
		}

		#endregion
	}
}