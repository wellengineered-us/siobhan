/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.UnitTests.Cli.Primitives._
{
	[TestFixture]
	public class AsyncYieldStateMachineTests
	{
		#region Constructors/Destructors

		public AsyncYieldStateMachineTests()
		{
		}

		#endregion

		#region Methods/Operators

		private static async IAsyncEnumerable<int> GetIntsAsync()
		{
			await Task.CompletedTask;
			for (int i = 1; i <= 16; i = i * 2)
				yield return i;
		}

		[Test]
		public async ValueTask ShouldCreateTestAsync()
		{
			IAsyncEnumerable<int> asyncEnumerable;

			asyncEnumerable = new AsyncIntegerYielder();

			Assert.IsNotNull(asyncEnumerable);

			await foreach (int i in asyncEnumerable)
			{
				await Console.Out.WriteLineAsync(i.ToString());
			}
		}

		[Test]
		public async ValueTask ShouldForEachTestAsync()
		{
			IAsyncEnumerable<string> asyncEnumerable = new AsyncForEachLifecycleYieldStateMachine<int, string>(GetIntsAsync(), async (index, item) =>
																																{
																																	await Task.CompletedTask;
																																	return item.ToString("00");
																																});

			Assert.IsNotNull(asyncEnumerable);

			await foreach (string s in asyncEnumerable)
			{
				await Console.Out.WriteLineAsync(s);
			}
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public sealed class AsyncIntegerYielder : AsyncLifecycleYieldStateMachine<int>
		{
			#region Constructors/Destructors

			public AsyncIntegerYielder()
			{
			}

			#endregion

			#region Fields/Constants

			readonly int lb = 0;
			readonly int ub = 10;
			private int value;

			#endregion

			#region Methods/Operators

			protected override async ValueTask CoreCreateAsync(bool creating, CancellationToken cancellationToken = default)
			{
				await Console.Out.WriteLineAsync("create");
			}

			protected override async ValueTask CoreDisposeAsync(bool disposing, CancellationToken cancellationToken = default)
			{
				await Console.Out.WriteLineAsync("dispose");
			}

			protected override IAsyncLifecycleEnumerator<int> CoreNewAsyncLifecycleEnumerator(int machineState, CancellationToken cancellationToken = default)
			{
				return this;
			}

			protected override async ValueTask<Tuple<bool, int>> CoreOnTryYieldAsync(CancellationToken cancellationToken = default)
			{
				Console.Out.WriteLine("try");

				if (this.value < this.ub)
				{
					//if(this.value == 5)
					//	throw new InvalidOperationException();

					return new Tuple<bool, int>(true, this.value);
				}

				await Task.CompletedTask;
				return new Tuple<bool, int>(default, default);
			}

			protected override async ValueTask CoreOnYieldCompleteAsync(CancellationToken cancellationToken = default)
			{
				await Console.Out.WriteLineAsync("complete");
				this.value = -1; // }
			}

			protected override async ValueTask CoreOnYieldFaultAsync(Exception ex, CancellationToken cancellationToken = default)
			{
				await Console.Out.WriteLineAsync("fault");
			}

			protected override async ValueTask CoreOnYieldResumeAsync(CancellationToken cancellationToken = default)
			{
				await Console.Out.WriteLineAsync("resume");
			}

			protected override async ValueTask CoreOnYieldReturnAsync(CancellationToken cancellationToken = default)
			{
				await Console.Out.WriteLineAsync("return");
				this.value++; // for(..., ..., value++)
			}

			protected override async ValueTask CoreOnYieldStartAsync(CancellationToken cancellationToken = default)
			{
				await Console.Out.WriteLineAsync("start");
				this.value = this.lb; // for(int value = lb; ...
			}

			#endregion
		}

		#endregion
	}
}
#endif