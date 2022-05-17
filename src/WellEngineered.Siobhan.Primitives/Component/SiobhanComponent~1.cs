/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Component;

namespace WellEngineered.Siobhan.Primitives.Component
{
	public abstract class SiobhanComponent<TSiobhanConfiguration>
		: SolderComponent<TSiobhanConfiguration>,
			ISiobhanComponent<TSiobhanConfiguration>
		where TSiobhanConfiguration : class, ISiobhanConfiguration
	{
	}
}