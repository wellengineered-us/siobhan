/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Solder.Component;
using WellEngineered.Solder.Configuration;

namespace WellEngineered.Siobhan.Primitives.Configuration
{
	public class UnknownSiobhanConfiguration<TSiobhanSpecification>
		: UnknownSolderConfiguration<TSiobhanSpecification>,
			IUnknownSiobhanConfiguration<TSiobhanSpecification>
		where TSiobhanSpecification : class, ISiobhanSpecification, new()
	{
		#region Constructors/Destructors

		public UnknownSiobhanConfiguration(IUnknownSolderConfiguration that)
			: base(that)
		{
		}

		#endregion
	}
}