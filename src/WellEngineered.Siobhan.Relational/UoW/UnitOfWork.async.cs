/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Relational.UoW
{
	/// <summary>
	/// Represents an atomic set of data operations on a single connection/transaction.
	/// </summary>
	public sealed partial class UnitOfWork
		: DualLifecycle,
			IUnitOfWork
	{
		#region Properties/Indexers/Events

		/// <summary>
		/// Gets a value indicating whether the current instance has been completed.
		/// </summary>
		public bool IsAsyncCompleted
		{
			get
			{
				return this.isCompletedDual;
			}
			private set
			{
				this.isCompletedDual = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current instance has been diverged.
		/// </summary>
		public bool IsAsyncDiverged
		{
			get
			{
				return this.isDivergedDual;
			}
			private set
			{
				this.isDivergedDual = value;
			}
		}

		#endregion

		#region Methods/Operators

		/// <summary>
		/// Creates a new unit of work (and opens the underlying connection) for the given connection type and connection string with an optional transaction started.
		/// </summary>
		/// <param name="connectionType"> The run-time type of the connection to use. </param>
		/// <param name="connectionString"> The ADO.NET provider connection string to use. </param>
		/// <param name="transactional"> A value indicating whether a new local data source transaction isstarted on the connection. </param>
		/// <param name="isolationLevel"> A value indicating the transaction isolation level. </param>
		/// <returns> An instance of teh unitOfWork ready for execution of operations. This should be wrapped in a using(...){} block for an optimal usage scenario. </returns>
		public static async ValueTask<IUnitOfWork> CreateAsync(Type connectionType, string connectionString, bool transactional, IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
		{
			UnitOfWork unitOfWork;
			DbConnection dbConnection;
			DbTransaction dbTransaction;
			const bool OPEN = true;

			if ((object)connectionType == null)
				throw new ArgumentNullException(nameof(connectionType));

			if ((object)connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			dbConnection = (DbConnection)Activator.CreateInstance(connectionType);

			if (OPEN)
			{
				dbConnection.ConnectionString = connectionString;
				await dbConnection.OpenAsync(cancellationToken);

				if (transactional)
					dbTransaction = await dbConnection.BeginTransactionAsync(isolationLevel, cancellationToken);
				else
					dbTransaction = null;
			}

			unitOfWork = new UnitOfWork(dbConnection, dbTransaction);

			return unitOfWork;
		}

		private async ValueTask AdjudicateAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				if ((object)this.Transaction != null)
				{
					if (this.IsAsyncCompleted && !this.IsAsyncDiverged)
						await this.Transaction.CommitAsync(cancellationToken);
					else
						await this.Transaction.RollbackAsync(cancellationToken);
				}
			}
			finally
			{
				// destroy and tear-down the context
				if ((object)this.Context != null)
					await this.Context.SafeDisposeAsync(cancellationToken);

				// destroy and tear-down the transaction
				if ((object)this.Transaction != null)
					await this.Transaction.DisposeAsync();

				// destroy and tear-down the connection
				if ((object)this.Connection != null)
					await this.Connection.DisposeAsync();
			}
		}

		/// <summary>
		/// Indicates that all operations within the unit of work have completed successfully. This method should only be called once.
		/// </summary>
		public async ValueTask CompleteAsync(CancellationToken cancellationToken = default)
		{
			if (this.IsAsyncDisposed)
				throw new ObjectDisposedException(typeof(UnitOfWork).FullName);

			if (this.IsAsyncCompleted)
				throw new InvalidOperationException(string.Format("The current unit of work is already complete. You should dispose of the unit of work."));

			this.IsAsyncCompleted = true;
			await Task.CompletedTask;
		}

		protected override ValueTask CoreCreateAsync(bool creating, CancellationToken cancellationToken = default)
		{
			// do nothing
			return default;
		}

		protected override async ValueTask CoreDisposeAsync(bool disposing, CancellationToken cancellationToken = default)
		{
			if (disposing)
			{
				await this.AdjudicateAsync(cancellationToken);
			}
		}

		/// <summary>
		/// Indicates that at least one operation within the unit of work cause a failure in data concurrency or nullipotency. This forces the entire unit of work to yield an incomplete status. This method can be called any number of times.
		/// </summary>
		public async ValueTask DivergentAsync(CancellationToken cancellationToken = default)
		{
			if (this.IsAsyncDisposed)
				throw new ObjectDisposedException(typeof(UnitOfWork).FullName);

			this.IsAsyncDiverged = true;
			await Task.CompletedTask;
		}

		#endregion
	}
}
#endif