/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Model
{
	public sealed class SiobhanOffset : Dictionary<string, object>, ISiobhanOffset
	{
		#region Constructors/Destructors

		private SiobhanOffset()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		#endregion

		#region Fields/Constants

		private static readonly SiobhanOffset none = new SiobhanOffset();

		#endregion

		#region Properties/Indexers/Events

		public static SiobhanOffset None
		{
			get
			{
				return none;
			}
		}

		#endregion
	}
}