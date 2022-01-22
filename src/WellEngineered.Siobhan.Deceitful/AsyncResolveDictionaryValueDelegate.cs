/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;

namespace WellEngineered.Siobhan.Deceitful
{
	public delegate ValueTask<object> AsyncResolveDictionaryValueDelegate(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, CancellationToken cancellationToken = default);
}