/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Model
{
	public sealed class SiobhanPartition : Dictionary<string, object>, ISiobhanPartition
	{
		#region Constructors/Destructors

		private SiobhanPartition()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly SiobhanPartition none = new SiobhanPartition();

		#endregion

		#region Properties/Indexers/Events

		public static SiobhanPartition None
		{
			get
			{
				return none;
			}
		}

		#endregion
	}
}