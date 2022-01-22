/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using WellEngineered.Solder.Primitives;

using static WellEngineered.Siobhan.Primitives.EnumerableExtensions;

namespace WellEngineered.Siobhan.Primitives
{
	public abstract class LifecycleYieldStateMachine<T>
		: Lifecycle,
			ILifecycleEnumerable<T>,
			ILifecycleEnumerator<T>
	{
		#region Constructors/Destructors

		protected LifecycleYieldStateMachine()
			: this(ENUMERATOR_BEFORE_STATE)
		{
		}

		protected LifecycleYieldStateMachine(int machineState)
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

		private T Current
		{
			get
			{
				return this.CoreGetNext();
			}
		}

		T IEnumerator<T>.Current
		{
			get
			{
				return this.Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		protected long InitialManagedThreadId
		{
			get
			{
				return this.initialManagedThreadId;
			}
		}

		protected bool IsAbandonedIfNewlyIterated
		{
			get
			{
				return this.MachineState != ENUMERATOR_BEFORE_STATE;
			}
		}

		protected T CurrentItem
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

		public bool IsItenCached
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

		public long ItemIndex
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

		protected int MachineState
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

		#endregion

		#region Methods/Operators

		protected T CoreGetNext()
		{
			T value;

			if (!this.IsItenCached)
			{
				if (!this.CoreHasNext()) // next --> hasNext --> moveNext: not idempotent by design
					throw new InvalidOperationException(string.Format("Invalid yield state machine state: '{0}'.", nameof(this.CoreHasNext)));
			}

			value = this.CurrentItem;

			this.IsItenCached = false;
			this.CurrentItem = default;

			return value;
		}

		protected bool CoreHasNext()
		{
			if (this.IsItenCached) // is there a cached current item value ready to yield?
				return true;
			else if (this.MachineState == ENUMERATOR_AFTER_STATE ||
					this.MachineState == ENUMERATOR_RUNNING_STATE)
				return false;
			else if (this.MachineState == ENUMERATOR_BEFORE_STATE ||
					this.MachineState == ENUMERATOR_RESUME_STATE)
			{
				bool result = this.CoreMoveNext(); // hasNext --> moveNext: not idempotent by design
				return result;
			}
			else
				throw new InvalidOperationException(string.Format("Invalid yield state machine state: '{0}'.", this.MachineState));
		}

		private void CoreIncrementItemIndex()
		{
			this.ItemIndex += 1;
		}

		protected virtual void CoreMaybeCreateBeforeYield()
		{
			this.Create();
		}

		protected virtual void CoreMaybeDisposeAfterFault()
		{
			this.Dispose();
		}

		protected virtual void CoreMaybeDisposeAfterYield()
		{
			this.Dispose();
		}

		protected bool CoreMoveNext()
		{
			bool result;
			T value;

			try
			{
				while (this.MachineState == ENUMERATOR_BEFORE_STATE ||
						this.MachineState == ENUMERATOR_RESUME_STATE)
				{
					switch (this.MachineState)
					{
						case ENUMERATOR_BEFORE_STATE:
						{
							this.MachineState = ENUMERATOR_RUNNING_STATE;

							this.CoreMaybeCreateBeforeYield();

							this.ItemIndex = -1;
							this.CoreOnYieldStart();

							this.MachineState = ENUMERATOR_RESUME_STATE;
							break;
						}
						case ENUMERATOR_RESUME_STATE:
						{
							this.MachineState = ENUMERATOR_RUNNING_STATE;

							if (this.IsItenCached)
								throw new InvalidOperationException(string.Format("Invalid yield state machine state: '{0}'.", nameof(this.IsItenCached)));

							if (this.ItemIndex > -1) // ???
								this.CoreOnYieldResume();

							result = this.CoreOnTryYield(out value);

							if (result)
							{
								this.CoreIncrementItemIndex();
								this.CoreOnYieldReturn();

								// we should not have a previously cached value, so this makes absolute sense...
								this.IsItenCached = true;
								this.CurrentItem = value;

								this.MachineState = ENUMERATOR_RESUME_STATE;
							}
							else
							{
								this.CoreOnYieldComplete();

								this.IsItenCached = false;
								this.CurrentItem = default;
								this.CoreMaybeDisposeAfterYield();

								this.MachineState = ENUMERATOR_AFTER_STATE;
							}

							return result;
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.CoreOnYieldFault(ex);

				this.IsItenCached = false;
				this.CurrentItem = default;
				this.MachineState = ENUMERATOR_FAULT_STATE;
				this.CoreMaybeDisposeAfterFault();

				throw; // re-throw
			}

			throw new InvalidOperationException(string.Format("Invalid yield state machine state: '{0}'.", this.MachineState));
		}

		protected abstract ILifecycleEnumerator<T> CoreNewLifecycleEnumerator(int machineState);

		protected abstract bool CoreOnTryYield(out T value);

		protected abstract void CoreOnYieldComplete();

		protected abstract void CoreOnYieldFault(Exception ex);

		protected abstract void CoreOnYieldResume();

		protected abstract void CoreOnYieldReturn();

		protected abstract void CoreOnYieldStart();

		public IEnumerator<T> GetEnumerator()
		{
			if (Thread.CurrentThread.ManagedThreadId == this.InitialManagedThreadId &&
				this.MachineState == ENUMERABLE_BEFORE_ENUMERATOR_STATE)
			{
				this.MachineState = ENUMERATOR_BEFORE_STATE;
				return this;
			}

			return this.CoreNewLifecycleEnumerator(ENUMERATOR_BEFORE_STATE);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool MoveNext()
		{
			return this.CoreMoveNext();
		}

		public void Reset()
		{
			throw new InvalidOperationException(string.Format("Invalid async-yield state machine state: '{0}'.", this.MachineState));
		}

		#endregion
	}
}