/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

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

		ValueTask<bool> TryGetSurrogateValueAsync(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, out object surrogateValue, CancellationToken cancellationToken = default);

		#endregion
	}
}