/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

namespace WellEngineered.Siobhan.Model
{
	public interface ISiobhanSchemaBuilder
	{
		#region Methods/Operators

		SiobhanSiobhanSchemaBuilder AddField(string fieldName, Type fieldType, bool isFieldOptional, bool isFieldKeyPart, ISiobhanSchema fieldSchema = null);

		SiobhanSiobhanSchemaBuilder AddFields(IEnumerable<ISiobhanField> fields);

		ISiobhanSchema Build();

		SiobhanSiobhanSchemaBuilder WithName(string value);

		SiobhanSiobhanSchemaBuilder WithType(SiobhanSchemaType value);

		SiobhanSiobhanSchemaBuilder WithVersion(int value);

		#endregion
	}
}