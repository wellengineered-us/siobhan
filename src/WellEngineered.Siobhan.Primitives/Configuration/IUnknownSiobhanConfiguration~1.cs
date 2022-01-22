/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Solder.Configuration;

namespace WellEngineered.Siobhan.Primitives.Configuration
{
	public interface IUnknownSiobhanConfiguration<out TSiobhanSpecification>
		: IUnknownSolderConfiguration<TSiobhanSpecification>,
			ISiobhanConfiguration
		where TSiobhanSpecification : class, ISiobhanSpecification
	{
	}
}