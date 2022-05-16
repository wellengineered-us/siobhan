/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System.Collections.Generic;

using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Primitives
{
	public interface IAsyncLifecycleEnumerator<out T> : IAsyncLifecycle, IAsyncEnumerator<T>
	{
	}
}
#endif