/*
	Copyright ©2020-2022 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

#if ASYNC_ALL_THE_WAY_DOWN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WellEngineered.Siobhan.Deceitful.Configuration;
using WellEngineered.Siobhan.Model;
using WellEngineered.Siobhan.Primitives;
using WellEngineered.Siobhan.Primitives.Component;
using WellEngineered.Solder.Primitives;

namespace WellEngineered.Siobhan.Deceitful
{
	public sealed partial class ObfuscationEngine
		: SiobhanComponent<ObfuscationConfiguration>,
			IObfuscationEngine,
			IObfuscationContext
	{
		#region Fields/Constants

		private readonly AsyncResolveDictionaryValueDelegate asyncResolveDictionaryValueCallback;

		#endregion

		#region Properties/Indexers/Events

		private AsyncResolveDictionaryValueDelegate AsyncResolveDictionaryValueCallback
		{
			get
			{
				return this.asyncResolveDictionaryValueCallback;
			}
		}

		#endregion

		#region Methods/Operators

		private static async ValueTask<long?> GetHashAsync(long? multiplier, long? size, long? seed, object value, CancellationToken cancellationToken = default)
		{
			const long DEFAULT_HASH = -1L;
			long hashCode;
			byte[] buffer;
			Type valueType;
			string _value;

			await Task.CompletedTask;

			if ((object)multiplier == null)
				return null;

			if ((object)size == null)
				return null;

			if ((object)seed == null)
				return null;

			if (size == 0L)
				return null; // prevent DIV0

			if ((object)value == null)
				return null;

			valueType = value.GetType();

			if (valueType != typeof(String))
				return null;

			_value = (String)value;

			if (string.IsNullOrWhiteSpace(_value))
				return DEFAULT_HASH;

			_value = _value.Trim();

			buffer = Encoding.UTF8.GetBytes(_value);

			hashCode = (long)seed;
			for (int index = 0; index < buffer.Length; index++)
				hashCode = ((long)multiplier * hashCode + buffer[index]) % uint.MaxValue;

			if (hashCode > int.MaxValue)
				hashCode = hashCode - uint.MaxValue;

			if (hashCode < 0)
				hashCode = hashCode + int.MaxValue;

			hashCode = (hashCode % (long)size);

			return (int)hashCode;
		}

		private async ValueTask<object> _GetObfuscatedValueAsync(ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken = default)
		{
			IObfuscationStrategy obfuscationStrategy;
			ColumnConfiguration columnConfiguration;
			object obfuscatedFieldValue;

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			await Task.CompletedTask;

			if (!this.FieldCache.TryGetValue(field.FieldIndex, out columnConfiguration))
			{
				columnConfiguration = this.Configuration.TableConfiguration.ColumnConfigurations.SingleOrDefault(c => (c.ColumnName ?? string.Empty).Trim().ToLower() == (field.FieldName ?? string.Empty).Trim().ToLower());
				this.FieldCache.Add(field.FieldIndex, columnConfiguration);
			}

			if ((object)columnConfiguration == null)
				return originalFieldValue; // do nothing when no matching column spec

			if (string.IsNullOrEmpty(columnConfiguration.AssemblyQualifiedTypeName))
				return originalFieldValue; // do nothing when strategy type not set

			if (!this.ObfuscationStrategyCache.TryGetValue(columnConfiguration.AssemblyQualifiedTypeName, out obfuscationStrategy))
			{
				Type obfuscationStrategyType;

				obfuscationStrategyType = columnConfiguration.GetComponentType();

				if ((object)obfuscationStrategyType == null)
					return originalFieldValue; // do nothing when strategy type not instantiable

				obfuscationStrategy = obfuscationStrategyType.CreateInstanceAssignableToTargetType<IObfuscationStrategy>();

				if ((object)obfuscationStrategy == null)
					throw new InvalidOperationException(string.Format("Unknown obfuscation strategy '{0}' specified for column '{1}'.", columnConfiguration.AssemblyQualifiedTypeName, field.FieldName));

				this.ObfuscationStrategyCache.Add(columnConfiguration.AssemblyQualifiedTypeName, obfuscationStrategy);
			}

			obfuscatedFieldValue = await obfuscationStrategy.GetObfuscatedValueAsync(this, columnConfiguration, field, originalFieldValue, cancellationToken);

			return obfuscatedFieldValue;
		}

		protected override async ValueTask CoreDisposeAsync(bool disposing, CancellationToken cancellationToken = default)
		{
			await Task.CompletedTask;

			if (disposing)
			{
				this.SubstitutionCacheRoot.Clear();
				this.FieldCache.Clear();
			}
		}

		private async ValueTask<long> GetBoundedHashAsync(long? size, object value, CancellationToken cancellationToken = default)
		{
			long? hash;

			await Task.CompletedTask;

			hash = GetHash(this
					.Configuration
					.HashConfiguration
					.Multiplier,
				size,
				this
					.Configuration
					.HashConfiguration
					.Seed,
				value.ToStringEx());

			if ((object)hash == null)
				throw new InvalidOperationException(string.Format("Oxymoron engine failed to calculate a valid hash for input '{0}'.", value.ToStringEx(null, "<null>")));

			return hash.GetValueOrDefault();
		}

		public async ValueTask<object> GetObfuscatedValueAsync(ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken = default)
		{
			object value;

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			if ((object)originalFieldValue == DBNull.Value)
				originalFieldValue = null;

			if (this.Configuration.EnablePassThru ?? false)
				return originalFieldValue; // pass-thru (safety switch)

			value = await this._GetObfuscatedValueAsync(field, originalFieldValue);

			return value;
		}

		public IAsyncLifecycleEnumerable<ISiobhanPayload> GetObfuscatedValuesAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> records, CancellationToken cancellationToken = default)
		{
			return GetObfuscatedValuesAsync(records, cancellationToken).ToAsyncLifecycleEnumerable();

			async IAsyncEnumerable<ISiobhanPayload> GetObfuscatedValuesAsync(IAsyncEnumerable<ISiobhanPayload> records, [EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				string fieldName;
				Type fieldType;
				object originalFieldValue, obfuscatedFieldValue;
				bool isFieldOptional = true;

				if ((object)records == null)
					throw new ArgumentNullException(nameof(records));

				await foreach (ISiobhanPayload record in records.WithCancellation(cancellationToken))
				{
					SiobhanPayload obfuscatedSiobhanPayload = null;

					if ((object)record != null)
					{
						obfuscatedSiobhanPayload = new SiobhanPayload();

						foreach (KeyValuePair<string, object> item in record)
						{
							ISiobhanField field; // TODO: should be provided to constructor

							fieldName = item.Key;
							originalFieldValue = record[item.Key];
							fieldType = (originalFieldValue ?? new object()).GetType();

							field = new SiobhanField()
									{
										FieldName = fieldName
									};

							obfuscatedFieldValue = await this.GetObfuscatedValueAsync(field, originalFieldValue, cancellationToken);
							obfuscatedSiobhanPayload.Add(fieldName, obfuscatedFieldValue);
						}
					}

					yield return obfuscatedSiobhanPayload;
				}
			}
		}

		public async ValueTask<long> GetSignHashAsync(object value, CancellationToken cancellationToken = default)
		{
			long hash;

			hash = await this.GetBoundedHashAsync(DEFAULT_HASH_BUCKET_SIZE, value, cancellationToken);

			return hash;
		}

		public async ValueTask<long> GetValueHashAsync(long? size, object value, CancellationToken cancellationToken = default)
		{
			long hash;

			hash = await this.GetBoundedHashAsync(size ?? DEFAULT_HASH_BUCKET_SIZE, value, cancellationToken);

			return hash;
		}

		private async ValueTask<object> ResolveDictionaryValueAsync(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, CancellationToken cancellationToken = default)
		{
			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)surrogateKey == null)
				throw new ArgumentNullException(nameof(surrogateKey));

			if ((object)this.AsyncResolveDictionaryValueCallback == null)
				return null;

			return await this.AsyncResolveDictionaryValueCallback(dictionaryConfiguration, surrogateKey, cancellationToken);
		}

		public async ValueTask<Tuple<bool, object>> TryGetSurrogateValueAsync(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, CancellationToken cancellationToken = default)
		{
			IDictionary<object, object> dictionaryCache;
			object surrogateValue;

			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if (!this.Configuration.DisableEngineCaches ?? false)
			{
				if (!this.SubstitutionCacheRoot.TryGetValue(dictionaryConfiguration.DictionaryId, out dictionaryCache))
				{
					dictionaryCache = new Dictionary<object, object>();
					this.SubstitutionCacheRoot.Add(dictionaryConfiguration.DictionaryId, dictionaryCache);
				}

				if (!dictionaryCache.TryGetValue(surrogateKey, out surrogateValue))
				{
					if (dictionaryConfiguration.PreloadEnabled)
						throw new InvalidOperationException(string.Format("Cache miss when dictionary preload enabled for dictionary ID '{0}'; current cache slot item count: {1}.", dictionaryConfiguration.DictionaryId, dictionaryCache.Count));

					surrogateValue = await this.ResolveDictionaryValueAsync(dictionaryConfiguration, surrogateKey, cancellationToken);
					dictionaryCache.Add(surrogateKey, surrogateValue);
				}
			}
			else
			{
				surrogateValue = await this.ResolveDictionaryValueAsync(dictionaryConfiguration, surrogateKey, cancellationToken);
			}

			return new Tuple<bool, object>(true, surrogateValue); // fake 'out' parameter
		}

		#endregion
	}
}
#endif