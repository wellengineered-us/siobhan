/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data.Common;

using WellEngineered.Siobhan.Middleware;

namespace WellEngineered.Siobhan.Relational
{
	public class AdoNetStreamingDataReader : WrappedDbDataReader
	{
		#region Constructors/Destructors

		public AdoNetStreamingDataReader(DbDataReader innerDbDataReader)
			: base(innerDbDataReader)
		{
		}

		#endregion
	}
}