/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WellEngineered.Siobhan.Middleware.Streams.Internal
{
	public class ProgressWrappedStream : WrappedStream
	{
		#region Constructors/Destructors

		public ProgressWrappedStream(Stream innerStream)
			: base(innerStream)
		{
		}

		#endregion

		#region Fields/Constants

		private long total = 0;

		#endregion

		#region Properties/Indexers/Events

		private long Total
		{
			get
			{
				return this.total;
			}
			set
			{
				this.total = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			Console.WriteLine("CLOSE: total={0}", this.Total);
			base.Close();
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			this.Write("COPY_ASYNC");
			return base.CopyToAsync((destination), bufferSize, cancellationToken);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int retval;

			retval = base.Read(buffer, offset, count);

			this.Total += retval;
			this.Write("READ: offset={0}, count={1}; retval={2}; total={3}", offset, count, retval, this.Total);

			return retval;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			Task<int> retval = base.ReadAsync(buffer, offset, count, cancellationToken);

			this.Write("READ_ASYNC: offset={0}, count={1}; retval={2}; total={3}", offset, count, retval, this.Total);

			return retval;
		}

		private void Write(string message, params object[] formats)
		{
			//Console.WriteLine(message, formats);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			int retval;

			base.Write(buffer, offset, count);
			retval = count;

			this.Total += retval;
			this.Write("WRITE: offset={0}, count={1}; retval={2}; total={3}", offset, count, retval, this.Total);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			Task retval = base.WriteAsync(buffer, offset, count, cancellationToken);

			this.Write("WRITE_ASYNC: offset={0}, count={1}; retval={2}; total={3}", offset, count, retval, this.Total);

			return retval;
		}

		#endregion
	}
}