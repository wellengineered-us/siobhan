/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;

namespace WellEngineered.Siobhan.Middleware.Streams
{
	public abstract class StreamMiddleware : SiobhanMiddleware<Stream>
	{
		#region Constructors/Destructors

		protected StreamMiddleware()
		{
		}

		#endregion
	}
}