/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using WellEngineered.Siobhan.Deceitful.Configuration;

namespace WellEngineered.Siobhan.Deceitful
{
	public partial interface IObfuscationContext
	{
		#region Methods/Operators

		long GetSignHash(object value);

		long GetValueHash(long? size, object value);

		bool TryGetSurrogateValue(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, out object surrogateValue);

		#endregion
	}
}