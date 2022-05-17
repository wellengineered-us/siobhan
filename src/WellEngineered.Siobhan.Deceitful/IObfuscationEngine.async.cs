/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Siobhan.Primitives.Component;

namespace WellEngineered.Siobhan.Deceitful
{
	public partial interface IObfuscationEngine
		: ISiobhanComponent<ObfuscationConfiguration>
	{
		#region Methods/Operators

		ValueTask<object> GetObfuscatedValueAsync(ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken = default);

		IAsyncLifecycleEnumerable<ISiobhanPayload> GetObfuscatedValuesAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> asyncRecords, CancellationToken cancellationToken = default);

		#endregion
	}
}
#endif