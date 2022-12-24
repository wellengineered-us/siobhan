/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using WellEngineered.Solder.Component;

namespace WellEngineered.Siobhan.Primitives.Configuration
{
	public class UnknownSiobhanConfiguration
		: UnknownSolderConfiguration,
			IUnknownSiobhanConfiguration
	{
		#region Constructors/Destructors

		public UnknownSiobhanConfiguration()
			: this(new Dictionary<string, object>(), null)
		{
		}

		public UnknownSiobhanConfiguration(IDictionary<string, object> componentSpecificConfiguration, Type componentSpecificConfigurationType)
			: base(componentSpecificConfiguration, componentSpecificConfigurationType)
		{
		}

		#endregion
	}
}