/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace WellEngineered.Siobhan.Model
{
	public sealed class SiobhanPayload : Dictionary<string, object>, ISiobhanPayload
	{
		#region Constructors/Destructors

		public SiobhanPayload()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		#endregion

		#region Methods/Operators

		public static SiobhanPayload FromPrimitive<T>(T value)
		{
			return FromPrimitive((object)value);
		}

		public static SiobhanPayload FromPrimitive(object value)
		{
			SiobhanPayload payload;

			payload = new SiobhanPayload();
			payload.Add(string.Empty, value);

			return payload;
		}

		public override string ToString()
		{
			return string.Join(", ", this.Select(kv => $"{kv.Key}={kv.Value}"));
		}

		#endregion
	}
}