/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational.UoW
{
	public partial interface IUnitOfWork
		: ILifecycle,
			IAsyncLifecycle
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value indicating whether the current instance has been completed.
		/// </summary>
		bool IsAsyncCompleted
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been diverged.
		/// </summary>
		bool IsAsyncDiverged
		{
			get;
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Indicates that all operations within the unit of work have completed successfully. This method should only be called once.
		/// </summary>
		ValueTask CompleteAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Indicates that at least one operation within the unit of work cause a failure in data concurrency or nullipotency. This forces the entire unit of work to yield an incomplete status. This method can be called any number of times.
		/// </summary>
		ValueTask DivergentAsync(CancellationToken cancellationToken = default);

		#endregion
	}
}