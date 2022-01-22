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
	public class AsyncForEachLifecycleYieldStateMachine<TInputItem, TOutputItem>
		: AsyncLifecycleYieldStateMachine<TOutputItem>
	{
		#region Constructors/Destructors

		public AsyncForEachLifecycleYieldStateMachine(IAsyncEnumerable<TInputItem> baseAsyncEnumerable, Func<long, TInputItem, ValueTask<TOutputItem>> asyncItemCallback)
			: this(ENUMERATOR_BEFORE_STATE, baseAsyncEnumerable, asyncItemCallback)
		{
		}

		protected AsyncForEachLifecycleYieldStateMachine(int machineState, IAsyncEnumerable<TInputItem> baseAsyncEnumerable, Func<long, TInputItem, ValueTask<TOutputItem>> asyncItemCallback)
			: base(machineState)
		{
			if ((object)baseAsyncEnumerable == null)
				throw new ArgumentNullException(nameof(baseAsyncEnumerable));

			if ((object)asyncItemCallback == null)
				throw new ArgumentNullException(nameof(asyncItemCallback));

			this.baseAsyncEnumerable = baseAsyncEnumerable;
			this.asyncItemCallback = asyncItemCallback;
		}

		#endregion

		#region Fields/Constants

		private readonly Func<long, TInputItem, ValueTask<TOutputItem>> asyncItemCallback;
		private readonly IAsyncEnumerable<TInputItem> baseAsyncEnumerable;
		private IAsyncEnumerator<TInputItem> baseAsyncEnumerator;

		#endregion

		#region Properties/Indexers/Events

		protected Func<long, TInputItem, ValueTask<TOutputItem>> AsyncItemCallback
		{
			get
			{
				return this.asyncItemCallback;
			}
		}

		protected IAsyncEnumerable<TInputItem> BaseAsyncEnumerable
		{
			get
			{
				return this.baseAsyncEnumerable;
			}
		}

		protected IAsyncEnumerator<TInputItem> BaseAsyncEnumerator
		{
			get
			{
				return this.baseAsyncEnumerator;
			}
			set
			{
				this.baseAsyncEnumerator = value;
			}
		}

		#endregion

		#region Methods/Operators

		protected override async ValueTask CoreCreateAsync(bool creating, CancellationToken cancellationToken = default)
		{
			if (this.IsAsyncCreated)
				return;

			if (creating)
			{
				if ((object)this.BaseAsyncEnumerable == null)
					throw new InvalidOperationException(nameof(this.BaseAsyncEnumerable));

				if ((object)this.BaseAsyncEnumerator != null)
					throw new InvalidOperationException(nameof(this.BaseAsyncEnumerator));

				await this.BaseAsyncEnumerable.SafeCreateAsync(cancellationToken);
			}
		}

		protected override async ValueTask CoreDisposeAsync(bool disposing, CancellationToken cancellationToken = default)
		{
			if (this.IsAsyncDisposed)
				return;

			if (disposing)
			{
				await this.BaseAsyncEnumerator.SafeDisposeAsync(cancellationToken);
				await this.BaseAsyncEnumerable.SafeDisposeAsync(cancellationToken);
			}
		}

		protected override IAsyncLifecycleEnumerator<TOutputItem> CoreNewAsyncLifecycleEnumerator(int machineState, CancellationToken cancellationToken = default)
		{
			// simple default implementation
			return this;
		}

		protected override async ValueTask<Tuple<bool, TOutputItem>> CoreOnTryYieldAsync(CancellationToken cancellationToken = default)
		{
			TInputItem oldItem;
			TOutputItem newItem;
			bool hasNext;

			if ((object)this.BaseAsyncEnumerator == null)
				throw new InvalidOperationException(nameof(this.BaseAsyncEnumerator));

			if ((object)this.AsyncItemCallback == null)
				throw new InvalidOperationException(nameof(this.AsyncItemCallback));

			hasNext = await this.BaseAsyncEnumerator.MoveNextAsync();

			if (!hasNext)
			{
				return new Tuple<bool, TOutputItem>(false, default);
			}

			oldItem = this.BaseAsyncEnumerator.Current;
			newItem = await this.AsyncItemCallback(this.AsyncItemIndex + 1 /* tentative index */, oldItem);

			return new Tuple<bool, TOutputItem>(true, newItem);
		}

		protected override async ValueTask CoreOnYieldCompleteAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				await this.BaseAsyncEnumerator.DisposeAsync();
			}
			finally
			{
				this.BaseAsyncEnumerator = null;
			}
		}

		protected override async ValueTask CoreOnYieldFaultAsync(Exception ex, CancellationToken cancellationToken = default)
		{
			try
			{
				await this.BaseAsyncEnumerator.DisposeAsync();
			}
			finally
			{
				this.BaseAsyncEnumerator = null;
			}
		}

		protected override async ValueTask CoreOnYieldResumeAsync(CancellationToken cancellationToken = default)
		{
			if ((object)this.BaseAsyncEnumerator == null)
				throw new InvalidOperationException(nameof(this.BaseAsyncEnumerator));

			await Task.CompletedTask;
		}

		protected override ValueTask CoreOnYieldReturnAsync(CancellationToken cancellationToken = default)
		{
			// do nothing
			return default;
		}

		protected override async ValueTask CoreOnYieldStartAsync(CancellationToken cancellationToken = default)
		{
			if ((object)this.BaseAsyncEnumerator != null)
				throw new InvalidOperationException(nameof(this.BaseAsyncEnumerator));

			this.BaseAsyncEnumerator = this.BaseAsyncEnumerable.GetAsyncEnumerator(cancellationToken);
			await this.BaseAsyncEnumerator.SafeCreateAsync(cancellationToken);
		}

		#endregion
	}
}
#endif