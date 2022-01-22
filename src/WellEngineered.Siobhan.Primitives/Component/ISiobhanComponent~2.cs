/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Primitives.Configuration;
using WellEngineered.Solder.Component;

namespace WellEngineered.Siobhan.Primitives.Component
{
	public interface ISiobhanComponent<TSiobhanConfiguration, TSiobhanSpecification>
		: ISiobhanComponent2,
			ISolderComponent<TSiobhanConfiguration, TSiobhanSpecification>
		where TSiobhanConfiguration : class, IUnknownSiobhanConfiguration<TSiobhanSpecification>
		where TSiobhanSpecification : class, ISiobhanSpecification, new()
	{
	}
}