/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;

namespace WellEngineered.Siobhan.Textual
{
	public static class TextualFieldTypeExtensions
	{
		#region Methods/Operators

		public static Type ToClrType(this TextualFieldType value)
		{
			return ToClrType((TextualFieldType?)value);
		}

		public static Type ToClrType(this TextualFieldType? value)
		{
			switch (value ?? TextualFieldType.Text) // default to string
			{
				case TextualFieldType.Text:
					return typeof(string);
				case TextualFieldType.Currency:
					return typeof(decimal?);
				case TextualFieldType.DateTime:
					return typeof(DateTimeOffset?);
				case TextualFieldType.Logical:
					return typeof(bool?);
				case TextualFieldType.Number:
					return typeof(double?);
				default:
					return typeof(object);
			}
		}

		#endregion
	}
}