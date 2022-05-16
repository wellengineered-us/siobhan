/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.IO.Compression;

namespace WellEngineered.Siobhan.Middleware.Streams
{
	public sealed class GzipCompressionStreamMiddleware : StreamMiddleware
	{
		#region Constructors/Destructors

		public GzipCompressionStreamMiddleware()
		{
		}

		#endregion

		#region Methods/Operators

		protected override Stream CoreApply(Stream target)
		{
			target = new GZipStream(target, CompressionLevel.Optimal);
			return target;
		}

		#endregion
	}
}