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

		SiobhanSchemaBuilder AddField(string fieldName, Type fieldType, bool isFieldOptional, bool isFieldKeyPart, ISiobhanSchema fieldSchema = null);

		SiobhanSchemaBuilder AddFields(IEnumerable<ISiobhanField> fields);

		ISiobhanSchema Build();

		SiobhanSchemaBuilder WithName(string value);

		SiobhanSchemaBuilder WithType(SiobhanSchemaType value);

		SiobhanSchemaBuilder WithVersion(int value);

		#endregion
	}
}