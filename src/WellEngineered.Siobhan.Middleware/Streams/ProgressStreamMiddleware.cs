/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

using WellEngineered.Siobhan.Middleware.Streams.Internal;

namespace WellEngineered.Siobhan.Middleware.Streams
{
	public sealed class ProgressStreamMiddleware : StreamMiddleware
	{
		#region Constructors/Destructors

		public ProgressStreamMiddleware()
		{
		}

		#endregion

		#region Methods/Operators

		protected override Stream CoreApply(Stream target)
		{
			if ((object)target == null)
				throw new ArgumentNullException(nameof(target));

			target = new ProgressWrappedStream(target);
			return target;
		}

		#endregion
	}
}