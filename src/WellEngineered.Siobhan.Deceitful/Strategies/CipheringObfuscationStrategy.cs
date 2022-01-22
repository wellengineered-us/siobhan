/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives.Configuration;

namespace WellEngineered.Siobhan.Deceitful.Strategies
{
	/// <summary>
	/// Returns an alternate value that is a binary encryption of the original value.
	/// DATA TYPE: string
	/// </summary>
	public sealed partial class CipheringObfuscationStrategy : ObfuscationStrategy<CipheringObfuscationStrategy.Spec>
	{
		#region Constructors/Destructors

		public CipheringObfuscationStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static object GetCipher(string sharedSecret, object value)
		{
			const string INIT_VECTOR = "0123456701234567";
			const int KEY_SIZE = 256;

			byte[] initVectorBytes;
			byte[] plainTextBytes;
			ICryptoTransform encryptor;
			byte[] keyBytes;
			byte[] cipherTextBytes;
			Type valueType;
			string _value;

			if ((object)sharedSecret == null)
				throw new ArgumentNullException(nameof(sharedSecret));

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (string.IsNullOrWhiteSpace(_value))
				return _value;

			_value = _value.Trim();

			initVectorBytes = Encoding.UTF8.GetBytes(INIT_VECTOR);
			plainTextBytes = Encoding.UTF8.GetBytes(_value);

			using (DeriveBytes password = new Rfc2898DeriveBytes(sharedSecret, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }))
			{
				keyBytes = password.GetBytes(KEY_SIZE / 8);
			}

			using (SymmetricAlgorithm symmetricKey = Rijndael.Create())
			{
				symmetricKey.Mode = CipherMode.CBC;
				encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
						cryptoStream.FlushFinalBlock();
						cipherTextBytes = memoryStream.ToArray();
					}
				}
			}

			return Encoding.UTF8.GetString(cipherTextBytes);
		}

		protected override object CoreGetObfuscatedValue(IObfuscationContext obfuscationContext, ColumnConfiguration columnConfiguration, ISiobhanField field, object originalFieldValue)
		{
			long signHash, valueHash;
			object value;
			string sharedSecret;

			if ((object)obfuscationContext == null)
				throw new ArgumentNullException(nameof(obfuscationContext));

			if ((object)columnConfiguration == null)
				throw new ArgumentNullException(nameof(columnConfiguration));

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			signHash = obfuscationContext.GetSignHash(originalFieldValue);
			valueHash = obfuscationContext.GetValueHash(null, originalFieldValue);
			sharedSecret = ((valueHash <= 0 ? 1 : valueHash) * (signHash % 2 == 0 ? -1 : 1)).ToString("X");

			value = GetCipher(sharedSecret, originalFieldValue);

			return value;
		}

		#endregion

		#region Classes/Structs/Interfaces/Enums/Delegates

		public sealed class Spec : SiobhanSpecification
		{
			#region Constructors/Destructors

			public Spec()
			{
			}

			#endregion
		}

		#endregion
	}
}