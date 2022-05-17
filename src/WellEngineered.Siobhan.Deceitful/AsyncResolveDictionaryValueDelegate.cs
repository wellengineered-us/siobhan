/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;

namespace WellEngineered.Siobhan.Deceitful
{
	public delegate ValueTask<object> AsyncResolveDictionaryValueDelegate(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, CancellationToken cancellationToken = default);
}
#endif