/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WellEngineered.Siobhan.Middleware
{
	public class WrappedTextReader : TextReader
	{
		#region Constructors/Destructors

		public WrappedTextReader(TextReader innerTextReader)
		{
			if ((object)innerTextReader == null)
				throw new ArgumentNullException(nameof(innerTextReader));

			this.innerTextReader = innerTextReader;
		}

		#endregion

		#region Fields/Constants

		private readonly TextReader innerTextReader;

		#endregion

		#region Properties/Indexers/Events

		protected TextReader InnerTextReader
		{
			get
			{
				return this.innerTextReader;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			this.InnerTextReader.Close();
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			// may not need this in .NET Core v2.0
			base.Dispose(disposing);
		}

		public override int Peek()
		{
			return this.InnerTextReader.Peek();
		}

		public override int Read(Span<char> buffer)
		{
			return this.InnerTextReader.Read(buffer);
		}

		public override int Read()
		{
			return this.InnerTextReader.Read();
		}

		public override int Read(char[] buffer, int index, int count)
		{
			return this.InnerTextReader.Read(buffer, index, count);
		}

		public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = new CancellationToken())
		{
			return this.InnerTextReader.ReadAsync(buffer, cancellationToken);
		}

		public override Task<int> ReadAsync(char[] buffer, int index, int count)
		{
			return this.InnerTextReader.ReadAsync(buffer, index, count);
		}

		public override int ReadBlock(Span<char> buffer)
		{
			return this.InnerTextReader.ReadBlock(buffer);
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			return this.InnerTextReader.ReadBlock(buffer, index, count);
		}

		public override ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = new CancellationToken())
		{
			return this.InnerTextReader.ReadBlockAsync(buffer, cancellationToken);
		}

		public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
		{
			return this.InnerTextReader.ReadBlockAsync(buffer, index, count);
		}

		public override string ReadLine()
		{
			return this.InnerTextReader.ReadLine();
		}

		public override Task<string> ReadLineAsync()
		{
			return this.InnerTextReader.ReadLineAsync();
		}

		public override string ReadToEnd()
		{
			return this.InnerTextReader.ReadToEnd();
		}

		public override Task<string> ReadToEndAsync()
		{
			return this.InnerTextReader.ReadToEndAsync();
		}

		#endregion
	}
}