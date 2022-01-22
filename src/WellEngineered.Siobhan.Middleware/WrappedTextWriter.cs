/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WellEngineered.Siobhan.Middleware
{
	public class WrappedTextWriter : TextWriter
	{
		#region Constructors/Destructors

		public WrappedTextWriter(TextWriter innerTextWriter)
		{
			if ((object)innerTextWriter == null)
				throw new ArgumentNullException(nameof(innerTextWriter));

			this.innerTextWriter = innerTextWriter;
		}

		#endregion

		#region Fields/Constants

		private readonly TextWriter innerTextWriter;

		#endregion

		#region Properties/Indexers/Events

		public override Encoding Encoding
		{
			get
			{
				return this.InnerTextWriter.Encoding;
			}
		}

		public override IFormatProvider FormatProvider
		{
			get
			{
				return this.InnerTextWriter.FormatProvider;
			}
		}

		protected TextWriter InnerTextWriter
		{
			get
			{
				return this.innerTextWriter;
			}
		}

		public override string NewLine
		{
			get
			{
				return this.InnerTextWriter.NewLine;
			}
			set
			{
				this.InnerTextWriter.NewLine = value;
			}
		}

		#endregion

		#region Methods/Operators

		public override void Close()
		{
			this.InnerTextWriter.Close();
			GC.SuppressFinalize(this);
		}

		protected override void Dispose(bool disposing)
		{
			// may not need this in .NET Core v2.0
			base.Dispose(disposing);
		}

		public override ValueTask DisposeAsync()
		{
			return this.InnerTextWriter.DisposeAsync();
		}

		public override void Flush()
		{
			this.InnerTextWriter.Flush();
		}

		public override Task FlushAsync()
		{
			return this.InnerTextWriter.FlushAsync();
		}

		public override void Write(ReadOnlySpan<char> buffer)
		{
			this.InnerTextWriter.Write(buffer);
		}

		public override void Write(bool value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(char value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(char[] buffer)
		{
			this.InnerTextWriter.Write(buffer);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			this.InnerTextWriter.Write(buffer, index, count);
		}

		public override void Write(decimal value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(double value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(float value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(int value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(long value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(object value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(string value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(string format, object arg0)
		{
			this.InnerTextWriter.Write(format, arg0);
		}

		public override void Write(string format, object arg0, object arg1)
		{
			this.InnerTextWriter.Write(format, arg0, arg1);
		}

		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			this.InnerTextWriter.Write(format, arg0, arg1, arg2);
		}

		public override void Write(string format, params object[] arg)
		{
			this.InnerTextWriter.Write(format, arg);
		}

		public override void Write(uint value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override void Write(ulong value)
		{
			this.InnerTextWriter.Write(value);
		}

		public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new CancellationToken())
		{
			return this.InnerTextWriter.WriteAsync(buffer, cancellationToken);
		}

		public override Task WriteAsync(char value)
		{
			return this.InnerTextWriter.WriteAsync(value);
		}

		public override Task WriteAsync(char[] buffer, int index, int count)
		{
			return this.InnerTextWriter.WriteAsync(buffer, index, count);
		}

		public override Task WriteAsync(string value)
		{
			return this.InnerTextWriter.WriteAsync(value);
		}

		public override void WriteLine(ReadOnlySpan<char> buffer)
		{
			this.InnerTextWriter.WriteLine(buffer);
		}

		public override void WriteLine()
		{
			this.InnerTextWriter.WriteLine();
		}

		public override void WriteLine(bool value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(char value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(char[] buffer)
		{
			this.InnerTextWriter.WriteLine(buffer);
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			this.InnerTextWriter.WriteLine(buffer, index, count);
		}

		public override void WriteLine(decimal value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(double value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(float value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(int value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(long value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(object value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(string value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(string format, object arg0)
		{
			this.InnerTextWriter.WriteLine(format, arg0);
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			this.InnerTextWriter.WriteLine(format, arg0, arg1);
		}

		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			this.InnerTextWriter.WriteLine(format, arg0, arg1, arg2);
		}

		public override void WriteLine(string format, params object[] arg)
		{
			this.InnerTextWriter.WriteLine(format, arg);
		}

		public override void WriteLine(uint value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override void WriteLine(ulong value)
		{
			this.InnerTextWriter.WriteLine(value);
		}

		public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = new CancellationToken())
		{
			return this.InnerTextWriter.WriteLineAsync(buffer, cancellationToken);
		}

		public override Task WriteLineAsync()
		{
			return this.InnerTextWriter.WriteLineAsync();
		}

		public override Task WriteLineAsync(char value)
		{
			return this.InnerTextWriter.WriteLineAsync(value);
		}

		public override Task WriteLineAsync(char[] buffer, int index, int count)
		{
			return this.InnerTextWriter.WriteLineAsync(buffer, index, count);
		}

		public override Task WriteLineAsync(string value)
		{
			return this.InnerTextWriter.WriteLineAsync(value);
		}

		#endregion
	}
}