/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

using WellEngineered.Siobhan.Primitives;

namespace WellEngineered.Siobhan.Middleware
{
	public abstract class SiobhanMiddleware<TTarget> : ISiobhanMiddleware<TTarget>
	{
		#region Constructors/Destructors

		protected SiobhanMiddleware()
		{
		}

		#endregion

		#region Methods/Operators

		public TTarget Apply(TTarget target)
		{
			if ((object)target == null)
				throw new ArgumentNullException(nameof(target));

			try
			{
				return this.CoreApply(target);
			}
			catch (Exception ex)
			{
				throw new SiobhanException(string.Format("The middleware failed (see inner exception)."), ex);
			}
		}

		protected abstract TTarget CoreApply(TTarget target);

		#endregion
	}
}