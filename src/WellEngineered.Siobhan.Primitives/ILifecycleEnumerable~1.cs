/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Primitives
{
	public interface ILifecycleEnumerable<out T> : ILifecycleEnumerable, IEnumerable<T>
	{
	}
}