/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

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

		object GetObfuscatedValue(ISiobhanField field, object originalFieldValue);

		ILifecycleEnumerable<ISiobhanPayload> GetObfuscatedValues(ILifecycleEnumerable<ISiobhanPayload> records);

		#endregion
	}
}