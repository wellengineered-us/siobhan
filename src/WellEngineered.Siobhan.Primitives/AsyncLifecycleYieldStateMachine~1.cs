/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Solder.Primitives;

using static WellEngineered.Siobhan.Primitives.EnumerableExtensions;

namespace WellEngineered.Siobhan.Primitives
{
	public abstract class AsyncLifecycleYieldStateMachine<T>
		: AsyncLifecycle,
			IAsyncLifecycleEnumerable<T>,
			IAsyncLifecycleEnumerator<T>
	{
		#region Constructors/Destructors

		protected AsyncLifecycleYieldStateMachine()
			: this(ENUMERATOR_BEFORE_STATE)
		{
		}

		protected AsyncLifecycleYieldStateMachine(int machineState)
		{
			this.machineState = machineState;
			this.initialManagedThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		#endregion

		#region Fields/Constants

		private readonly long initialManagedThreadId;
		private T currentItem;
		private bool isItenCached;
		private long itemIndex;
		private int machineState;

		#endregion

		#region Properties/Indexers/Events

		private T AsyncCurrent
		{
			get
			{
				// WTF
				return this.CoreGetNextAsync()
					.GetAwaiter()
					.GetResult();
			}
		}

		protected long AsyncInitialManagedThreadId
		{
			get
			{
				return this.initialManagedThreadId;
			}
		}

		T IAsyncEnumerator<T>.Current
		{
			get
			{
				return this.AsyncCurrent;
			}
		}

		protected bool IsAsyncAbandonedIfNewlyIterated
		{
			get
			{
				return this.AsyncMachineState != ENUMERATOR_BEFORE_STATE;
			}
		}

		protected T AsyncCurrentItem
		{
			get
			{
				return this.currentItem;
			}
			private set
			{
				this.currentItem = value;
			}
		}

		public long AsyncItemIndex
		{
			get
			{
				return this.itemIndex;
			}
			protected set
			{
				this.itemIndex = value;
			}
		}

		protected int AsyncMachineState
		{
			get
			{
				return this.machineState;
			}
			private set
			{
				this.machineState = value;
			}
		}

		public bool IsAsyncItenCached
		{
			get
			{
				return this.isItenCached;
			}
			protected set
			{
				this.isItenCached = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected async ValueTask<T> CoreGetNextAsync(CancellationToken cancellationToken = default)
		{
			T value;

			if (!this.IsAsyncItenCached)
			{
				if (!await this.CoreHasNextAsync(cancellationToken)) // next --> hasNext --> moveNext: not idempotent by design
					throw new InvalidOperationException(string.Format("Invalid async-yield state machine state: '{0}'.", nameof(this.CoreGetNextAsync)));
			}

			value = this.AsyncCurrentItem;

			this.IsAsyncItenCached = false;
			this.AsyncCurrentItem = default;

			return value;
		}

		protected async ValueTask<bool> CoreHasNextAsync(CancellationToken cancellationToken = default)
		{
			if (this.IsAsyncItenCached) // is there a cached current item value ready to yield?
				return true;
			else if (this.AsyncMachineState == ENUMERATOR_AFTER_STATE ||
					this.AsyncMachineState == ENUMERATOR_RUNNING_STATE)
				return false;
			else if (this.AsyncMachineState == ENUMERATOR_BEFORE_STATE ||
					this.AsyncMachineState == ENUMERATOR_RESUME_STATE)
			{
				bool result = await this.CoreMoveNextAsync(cancellationToken); // hasNext --> moveNext: not idempotent by design
				return result;
			}
			else
				throw new InvalidOperationException(string.Format("Invalid async-yield state machine state: '{0}'.", this.AsyncMachineState));
		}

		private async ValueTask CoreIncrementItemIndexAsync(CancellationToken cancellationToken = default)
		{
			this.AsyncItemIndex += 1;
			await Task.CompletedTask;
		}

		protected virtual async ValueTask CoreMaybeCreateBeforeYieldAsync(CancellationToken cancellationToken = default)
		{
			await this.CreateAsync(cancellationToken);
		}

		protected virtual async ValueTask CoreMaybeDisposeAfterFaultAsync(CancellationToken cancellationToken = default)
		{
			await this.DisposeAsync(cancellationToken);
		}

		protected virtual async ValueTask CoreMaybeDisposeAfterYieldAsync(CancellationToken cancellationToken = default)
		{
			await this.DisposeAsync(cancellationToken);
		}

		protected async ValueTask<bool> CoreMoveNextAsync(CancellationToken cancellationToken = default)
		{
			bool result;
			T value;

			try
			{
				while (this.AsyncMachineState == ENUMERATOR_BEFORE_STATE ||
						this.AsyncMachineState == ENUMERATOR_RESUME_STATE)
				{
					switch (this.AsyncMachineState)
					{
						case ENUMERATOR_BEFORE_STATE:
						{
							this.AsyncMachineState = ENUMERATOR_RUNNING_STATE;

							await this.CoreMaybeCreateBeforeYieldAsync(cancellationToken);

							this.AsyncItemIndex = -1;
							await this.CoreOnYieldStartAsync(cancellationToken);

							this.AsyncMachineState = ENUMERATOR_RESUME_STATE;
							break;
						}
						case ENUMERATOR_RESUME_STATE:
						{
							this.AsyncMachineState = ENUMERATOR_RUNNING_STATE;

							if (this.IsAsyncItenCached)
								throw new InvalidOperationException(string.Format("Invalid async-yield state machine state: '{0}'.", nameof(this.IsAsyncItenCached)));

							if (this.AsyncItemIndex > -1) // ???
								await this.CoreOnYieldResumeAsync(cancellationToken);

							Tuple<bool, T> snap = await this.CoreOnTryYieldAsync(cancellationToken);

							result = (object)snap != null && snap.Item1;
							value = (object)snap == null ? default : snap.Item2;

							if (result)
							{
								await this.CoreIncrementItemIndexAsync(cancellationToken);
								await this.CoreOnYieldReturnAsync(cancellationToken);

								// we should not have a previously cached value, so this makes absolute sense...
								this.IsAsyncItenCached = true;
								this.AsyncCurrentItem = value;

								this.AsyncMachineState = ENUMERATOR_RESUME_STATE;
							}
							else
							{
								await this.CoreOnYieldCompleteAsync(cancellationToken);

								this.IsAsyncItenCached = false;
								this.AsyncCurrentItem = default;
								await this.CoreMaybeDisposeAfterYieldAsync(cancellationToken);

								this.AsyncMachineState = ENUMERATOR_AFTER_STATE;
							}

							return result;
						}
					}
				}
			}
			catch (Exception ex)
			{
				await this.CoreOnYieldFaultAsync(ex);

				this.IsAsyncItenCached = false;
				this.AsyncCurrentItem = default;
				this.AsyncMachineState = ENUMERATOR_FAULT_STATE;
				await this.CoreMaybeDisposeAfterFaultAsync(cancellationToken);

				throw; // re-throw
			}

			throw new InvalidOperationException(string.Format("Invalid async-yield state machine state: '{0}'.", this.AsyncMachineState));
		}

		protected abstract IAsyncLifecycleEnumerator<T> CoreNewAsyncLifecycleEnumerator(int machineState, CancellationToken cancellationToken = default);

		protected abstract ValueTask<Tuple<bool, T>> CoreOnTryYieldAsync(CancellationToken cancellationToken = default);

		protected abstract ValueTask CoreOnYieldCompleteAsync(CancellationToken cancellationToken = default);

		protected abstract ValueTask CoreOnYieldFaultAsync(Exception ex, CancellationToken cancellationToken = default);

		protected abstract ValueTask CoreOnYieldResumeAsync(CancellationToken cancellationToken = default);

		protected abstract ValueTask CoreOnYieldReturnAsync(CancellationToken cancellationToken = default);

		protected abstract ValueTask CoreOnYieldStartAsync(CancellationToken cancellationToken = default);

		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			if (Thread.CurrentThread.ManagedThreadId == this.AsyncInitialManagedThreadId &&
				this.AsyncMachineState == ENUMERABLE_BEFORE_ENUMERATOR_STATE)
			{
				this.AsyncMachineState = ENUMERATOR_BEFORE_STATE;
				return this;
			}

			return this.CoreNewAsyncLifecycleEnumerator(ENUMERATOR_BEFORE_STATE, cancellationToken);
		}

		public ValueTask<bool> MoveNextAsync()
		{
			return this.CoreMoveNextAsync();
		}

		#endregion
	}
}
#endif