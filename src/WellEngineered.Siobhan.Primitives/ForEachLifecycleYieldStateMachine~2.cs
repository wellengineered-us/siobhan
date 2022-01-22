/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using WellEngineered.Solder.Primitives;

using static WellEngineered.Siobhan.Primitives.EnumerableExtensions;

namespace WellEngineered.Siobhan.Primitives
{
	public class ForEachLifecycleYieldStateMachine<TInputItem, TOutputItem>
		: LifecycleYieldStateMachine<TOutputItem>
	{
		#region Constructors/Destructors

		public ForEachLifecycleYieldStateMachine(IEnumerable<TInputItem> baseEnumerable, Func<long, TInputItem, TOutputItem> itemCallback)
			: this(ENUMERATOR_BEFORE_STATE, baseEnumerable, itemCallback)
		{
		}

		protected ForEachLifecycleYieldStateMachine(int machineState, IEnumerable<TInputItem> baseEnumerable, Func<long, TInputItem, TOutputItem> itemCallback)
			: base(machineState)
		{
			if ((object)baseEnumerable == null)
				throw new ArgumentNullException(nameof(baseEnumerable));

			if ((object)itemCallback == null)
				throw new ArgumentNullException(nameof(itemCallback));

			this.baseEnumerable = baseEnumerable;
			this.itemCallback = itemCallback;
		}

		#endregion

		#region Fields/Constants

		private readonly IEnumerable<TInputItem> baseEnumerable;
		private readonly Func<long, TInputItem, TOutputItem> itemCallback;
		private IEnumerator<TInputItem> baseEnumerator;

		#endregion

		#region Properties/Indexers/Events

		protected IEnumerable<TInputItem> BaseEnumerable
		{
			get
			{
				return this.baseEnumerable;
			}
		}

		protected Func<long, TInputItem, TOutputItem> ItemCallback
		{
			get
			{
				return this.itemCallback;
			}
		}

		protected IEnumerator<TInputItem> BaseEnumerator
		{
			get
			{
				return this.baseEnumerator;
			}
			set
			{
				this.baseEnumerator = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override void CoreCreate(bool creating)
		{
			if (this.IsCreated)
				return;

			if (creating)
			{
				if ((object)this.BaseEnumerable == null)
					throw new InvalidOperationException(nameof(this.BaseEnumerable));

				if ((object)this.BaseEnumerator != null)
					throw new InvalidOperationException(nameof(this.BaseEnumerator));

				this.BaseEnumerable.SafeCreate();
			}
		}

		protected override void CoreDispose(bool disposing)
		{
			if (this.IsDisposed)
				return;

			if (disposing)
			{
				this.BaseEnumerator.SafeDispose();
				this.BaseEnumerable.SafeDispose();
			}
		}

		protected override ILifecycleEnumerator<TOutputItem> CoreNewLifecycleEnumerator(int machineState)
		{
			// simple default implementation
			return this;
		}

		protected override bool CoreOnTryYield(out TOutputItem yielded)
		{
			TInputItem oldItem;
			TOutputItem newItem;
			bool hasNext;

			if ((object)this.BaseEnumerator == null)
				throw new InvalidOperationException(nameof(this.BaseEnumerator));

			if ((object)this.ItemCallback == null)
				throw new InvalidOperationException(nameof(this.ItemCallback));

			hasNext = this.BaseEnumerator.MoveNext();

			if (!hasNext)
			{
				yielded = default;
				return false;
			}

			oldItem = this.BaseEnumerator.Current;
			newItem = this.ItemCallback(this.ItemIndex + 1 /* tentative index */, oldItem);

			yielded = newItem;
			return true;
		}

		protected override void CoreOnYieldComplete()
		{
			try
			{
				this.BaseEnumerator.SafeDispose();
			}
			finally
			{
				this.BaseEnumerator = null;
			}
		}

		protected override void CoreOnYieldFault(Exception ex)
		{
			try
			{
				this.BaseEnumerator.SafeDispose();
			}
			finally
			{
				this.BaseEnumerator = null;
			}
		}

		protected override void CoreOnYieldResume()
		{
			if ((object)this.BaseEnumerator == null)
				throw new InvalidOperationException(nameof(this.BaseEnumerator));
		}

		protected override void CoreOnYieldReturn()
		{
			// do nothing
		}

		protected override void CoreOnYieldStart()
		{
			if ((object)this.BaseEnumerator != null)
				throw new InvalidOperationException(nameof(this.BaseEnumerator));

			this.BaseEnumerator = this.BaseEnumerable.GetEnumerator();
			this.BaseEnumerator.SafeCreate();
		}

		#endregion
	}
}