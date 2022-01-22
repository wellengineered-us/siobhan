/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace WellEngineered.Siobhan.Middleware.Streams
{
	public sealed class NoneStreamMiddleware : StreamMiddleware
	{
		#region Constructors/Destructors

		public NoneStreamMiddleware()
		{
		}

		#endregion

		#region Methods/Operators

		protected override Stream CoreApply(Stream target)
		{
			if ((object)target == null)
				throw new ArgumentNullException(nameof(target));

			return target;
		}

		#endregion
	}
}