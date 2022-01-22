/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using WellEngineered.Siobhan.Model;

namespace WellEngineered.Siobhan.Relational
{
	public static class AdoNetStreamingExtensionMethods
	{
		#region Methods/Operators

		public static ISiobhanPayload ToRecord(this IEnumerable<DbParameter> dbParameters)
		{
			SiobhanPayload record;

			if ((object)dbParameters == null)
				throw new ArgumentNullException(nameof(dbParameters));

			record = new SiobhanPayload();

			foreach (DbParameter dbParameter in dbParameters)
			{
				if (dbParameter.Direction != ParameterDirection.InputOutput &&
					dbParameter.Direction != ParameterDirection.Output &&
					dbParameter.Direction != ParameterDirection.ReturnValue)
					continue;

				record.Add(dbParameter.ParameterName, dbParameter.Value);
			}

			return record;
		}

		#endregion
	}
}