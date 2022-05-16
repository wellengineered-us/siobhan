/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Primitives.Component;
using WellEngineered.Siobhan.Primitives.Configuration;

namespace WellEngineered.Siobhan.Deceitful
{
	public interface IObfuscationStrategy<TObfuscationStrategySpecification>
		: IObfuscationStrategy,
			ISiobhanComponent<UnknownSiobhanConfiguration<TObfuscationStrategySpecification>, TObfuscationStrategySpecification>
		where TObfuscationStrategySpecification : class, ISiobhanSpecification, new()
	{
	}
}