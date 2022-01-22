/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.IO;

using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public partial interface ITextualReader<TTextualFieldSpec, out TTextualSpec> : ILifecycle
		where TTextualFieldSpec : ITextualFieldSpec
		where TTextualSpec : ITextualSpec<TTextualFieldSpec>
	{
		#region Properties/Indexers/Events

		TextReader BaseTextReader
		{
			get;
		}

		TTextualSpec TextualSpec
		{
			get;
		}

		#endregion

		#region Methods/Operators

		ILifecycleEnumerable<ITextualStreamingRecord> ReadFooterRecords(ILifecycleEnumerable<TTextualFieldSpec> footers);

		ILifecycleEnumerable<TTextualFieldSpec> ReadHeaderFields();

		ILifecycleEnumerable<ITextualStreamingRecord> ReadRecords();

		#endregion
	}
}