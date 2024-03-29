/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Primitives
{
	/// <summary>
	/// The exception thrown when a Siobhan runtime error occurs.
	/// </summary>
	public sealed class SiobhanException : Exception
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the SiobhanException class.
		/// </summary>
		public SiobhanException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SiobhanException class.
		/// </summary>
		/// <param name="message"> The message that describes the error. </param>
		public SiobhanException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SiobhanException class.
		/// </summary>
		/// <param name="message"> The message that describes the error. </param>
		/// <param name="innerException"> The inner exception. </param>
		public SiobhanException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}