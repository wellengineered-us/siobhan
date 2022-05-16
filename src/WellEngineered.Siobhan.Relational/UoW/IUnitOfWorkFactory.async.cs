/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational.UoW
{
	public partial interface IUnitOfWorkFactory
		: IAsyncLifecycle
	{
		#region Methods/Operators

		ValueTask<IUnitOfWork> GetUnitOfWorkAsync(bool transactional, IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default);

		#endregion
	}
}
#endif