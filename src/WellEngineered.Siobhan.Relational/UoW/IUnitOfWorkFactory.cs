/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Data;

using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational.UoW
{
	public partial interface IUnitOfWorkFactory
		: ILifecycle
	{
		#region Methods/Operators

		IUnitOfWork GetUnitOfWork(bool transactional, IsolationLevel isolationLevel = IsolationLevel.Unspecified);

		#endregion
	}
}