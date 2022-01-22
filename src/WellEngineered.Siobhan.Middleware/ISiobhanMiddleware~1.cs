/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Middleware
{
	public interface ISiobhanMiddleware<TTarget>
	{
		#region Methods/Operators

		TTarget Apply(TTarget target);

		#endregion
	}
}