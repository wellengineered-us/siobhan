/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;

namespace WellEngineered.Siobhan.Deceitful
{
	public partial interface IObfuscationContext
	{
		#region Methods/Operators

		ValueTask<long> GetSignHashAsync(object value, CancellationToken cancellationToken = default);

		ValueTask<long> GetValueHashAsync(long? size, object value, CancellationToken cancellationToken = default);

		ValueTask<Tuple<bool, object>> TryGetSurrogateValueAsync(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, CancellationToken cancellationToken = default);

		#endregion
	}
}
#endif