/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Primitives
{
	public static partial class EnumerableExtensions
	{
		#region Fields/Constants

		internal const int ENUMERABLE_BEFORE_ENUMERATOR_STATE = -2; // before the first call to GetEnumerator() from the creating thread
		internal const int ENUMERATOR_AFTER_STATE = -1; // "After" - the iterator has finished, either by reaching the end of the method or by hitting yield break
		internal const int ENUMERATOR_BEFORE_STATE = 0; // next() hasn't been called yet
		internal const int ENUMERATOR_FAULT_STATE = -1; // "Fault" - the iterator threw an exception - internal use only
		internal const int ENUMERATOR_RESUME_STATE = 1; // it's yielded at least one value, and there's possibly more to come
		internal const int ENUMERATOR_RUNNING_STATE = -1; // "Running" - the iterator is currently executing code

		#endregion
	}
}