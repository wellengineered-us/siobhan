/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using WellEngineered.Siobhan.Deceitful.Configuration;

namespace WellEngineered.Siobhan.Deceitful
{
	public delegate object ResolveDictionaryValueDelegate(DictionaryConfiguration dictionaryConfiguration, object surrogateKey);
}