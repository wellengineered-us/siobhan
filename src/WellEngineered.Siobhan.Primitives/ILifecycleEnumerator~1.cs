/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace WellEngineered.Siobhan.Primitives
{
	public interface ILifecycleEnumerator<out T> : ILifecycleEnumerator, IEnumerator<T>
	{
	}
}