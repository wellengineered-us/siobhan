/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Model
{
	public interface ISiobhanSchema
	{
		#region Properties/Indexers/Events

		IReadOnlyDictionary<string, ISiobhanField> Fields
		{
			get;
		}

		string SchemaName
		{
			get;
		}

		SiobhanSchemaType SchemaType
		{
			get;
		}

		int SchemaVersion
		{
			get;
		}

		#endregion
	}
}