/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using WellEngineered.Siobhan.Middleware;
using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.UnitTests.Cli.Primitives._
{
	[TestFixture]
	public class YieldStateMachineTests
	{
		#region Constructors/Destructors

		public YieldStateMachineTests()
		{
		}

		#endregion

		public sealed class IntegerYielder : LifecycleYieldStateMachine<int>
		{
			public IntegerYielder()
			{
			}

			readonly int lb = 0;
			readonly int ub = 10;
			private int value;
			
			protected override void CoreCreate(bool creating)
			{
				Console.Out.WriteLine("create");
			}

			protected override void CoreDispose(bool disposing)
			{
				Console.Out.WriteLine("dispose");
			}

			protected override ILifecycleEnumerator<int> CoreNewLifecycleEnumerator(int machineState)
			{
				return this;
			}

			protected override bool CoreOnTryYield(out int yielded)
			{
				Console.Out.WriteLine("try");

				if (this.value < this.ub)
				{
					//if(this.value == 5)
					//	throw new InvalidOperationException();

					yielded = this.value;
					return true;
				}

				yielded = default;
				return false;
			}

			protected override void CoreOnYieldComplete()
			{
				Console.Out.WriteLine("complete");
				this.value = -1; // }
			}

			protected override void CoreOnYieldFault(Exception ex)
			{
				Console.Out.WriteLine("fault");
			}

			protected override void CoreOnYieldResume()
			{
				Console.Out.WriteLine("resume");
			}

			protected override void CoreOnYieldReturn()
			{
				Console.Out.WriteLine("return");
				this.value++; // for(..., ..., value++)
			}

			protected override void CoreOnYieldStart()
			{
				Console.Out.WriteLine("start");
				this.value = this.lb; // for(int value = lb; ...
			}
		}

		#region Methods/Operators

		[Test]
		public void ShouldCreateTest()
		{
			IEnumerable<int> enumerable;

			enumerable = new IntegerYielder();
			
			Assert.IsNotNull(enumerable);

			foreach (int i in enumerable)
			{
				Console.WriteLine(i);
			}
		}
		
		[Test]
		public void ShouldForEachTest()
		{
			IEnumerable<string> enumerable = new ForEachLifecycleYieldStateMachine<int, string>(GetInts(), (index, item) => item.ToString("00"));
			
			Assert.IsNotNull(enumerable);

			foreach (string s in enumerable)
			{
				Console.WriteLine(s);
			}
		}

		private static IEnumerable<int> GetInts()
		{
			for (int i = 1; i <= 16; i = i * 2)
				yield return i;
		}

		#endregion
	}
}