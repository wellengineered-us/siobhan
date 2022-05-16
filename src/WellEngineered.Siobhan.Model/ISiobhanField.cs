/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Model
{
	public interface ISiobhanField
	{
		#region Properties/Indexers/Events

		long FieldIndex
		{
			get;
		}

		string FieldName
		{
			get;
		}

		ISiobhanSchema FieldSchema
		{
			get;
		}

		Type FieldType
		{
			get;
		}

		bool IsFieldKeyComponent
		{
			get;
		}

		bool IsFieldOptional
		{
			get;
		}

		#endregion
	}
}