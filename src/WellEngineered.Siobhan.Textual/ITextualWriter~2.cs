/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.IO;

using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Textual
{
	public partial interface ITextualWriter<in TTextualFieldSpec, out TTextualSpec>
		: ILifecycle
		where TTextualFieldSpec : ITextualFieldSpec
		where TTextualSpec : ITextualSpec<TTextualFieldSpec>
	{
		#region Properties/Indexers/Events

		TextWriter BaseTextWriter
		{
			get;
		}

		TTextualSpec TextualSpec
		{
			get;
		}

		#endregion

		#region Methods/Operators

		void Flush();

		void WriteFooterRecords(ILifecycleEnumerable<TTextualFieldSpec> specs, ILifecycleEnumerable<ITextualStreamingRecord> footers);

		void WriteHeaderFields(ILifecycleEnumerable<TTextualFieldSpec> specs);

		void WriteRecords(ILifecycleEnumerable<ISiobhanPayload /* SHOULD BE LCD INTERFACE */> records);

		#endregion
	}
}