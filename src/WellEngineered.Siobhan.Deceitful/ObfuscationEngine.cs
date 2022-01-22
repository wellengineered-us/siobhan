/*
	Copyright ©2020-2021 WellEngineered.us, all rights reserved.
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Linq;
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
	public sealed class ObfuscationEngine
		: SiobhanComponent<ObfuscationConfiguration>,
			IObfuscationEngine,
			IObfuscationContext
	{
		#region Constructors/Destructors

		public ObfuscationEngine(ResolveDictionaryValueDelegate resolveDictionaryValueCallback)
		{
			if ((object)resolveDictionaryValueCallback == null)
				throw new ArgumentNullException(nameof(resolveDictionaryValueCallback));

			this.resolveDictionaryValueCallback = resolveDictionaryValueCallback;
		}

		#endregion

		#region Fields/Constants

		private const long DEFAULT_HASH_BUCKET_SIZE = long.MaxValue;
		private const bool SUBSTITUTION_CACHE_ENABLED = true;
		private readonly IDictionary<long, ColumnConfiguration> fieldCache = new Dictionary<long, ColumnConfiguration>();
		private readonly IDictionary<string, IObfuscationStrategy> obfuscationStrategyCache = new Dictionary<string, IObfuscationStrategy>();
		private readonly ResolveDictionaryValueDelegate resolveDictionaryValueCallback;
		private readonly IDictionary<string, IDictionary<object, object>> substitutionCacheRoot = new Dictionary<string, IDictionary<object, object>>();

		#endregion

		#region Properties/Indexers/Events

		private IDictionary<long, ColumnConfiguration> FieldCache
		{
			get
			{
				return this.fieldCache;
			}
		}

		private IDictionary<string, IObfuscationStrategy> ObfuscationStrategyCache
		{
			get
			{
				return this.obfuscationStrategyCache;
			}
		}

		private ResolveDictionaryValueDelegate ResolveDictionaryValueCallback
		{
			get
			{
				return this.resolveDictionaryValueCallback;
			}
		}

		private IDictionary<string, IDictionary<object, object>> SubstitutionCacheRoot
		{
			get
			{
				return this.substitutionCacheRoot;
			}
		}

		#endregion

		#region Methods/Operators

		private static long? GetHash(long? multiplier, long? size, long? seed, object value)
		{
			const long DEFAULT_HASH = -1L;
			long hashCode;
			byte[] buffer;
			Type valueType;
			string _value;

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

		private object _GetObfuscatedValue(ISiobhanField field, object originalFieldValue)
		{
			IObfuscationStrategy obfuscationStrategy;
			ColumnConfiguration columnConfiguration;
			object obfuscatedFieldValue;

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

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

			obfuscatedFieldValue = obfuscationStrategy.GetObfuscatedValue(this, columnConfiguration, field, originalFieldValue);

			return obfuscatedFieldValue;
		}

		protected override void CoreDispose(bool disposing)
		{
			if (disposing)
			{
				this.SubstitutionCacheRoot.Clear();
				this.FieldCache.Clear();
			}
		}

		private long GetBoundedHash(long? size, object value)
		{
			long? hash;

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

		public object GetObfuscatedValue(ISiobhanField field, object columnValue)
		{
			object value;

			if ((object)field == null)
				throw new ArgumentNullException(nameof(field));

			if ((object)columnValue == DBNull.Value)
				columnValue = null;

			if (this.Configuration.EnablePassThru ?? false)
				return columnValue; // pass-thru (safety switch)

			value = this._GetObfuscatedValue(field, columnValue);

			return value;
		}

		public ValueTask<object> GetObfuscatedValueAsync(ISiobhanField field, object originalFieldValue, CancellationToken cancellationToken = default)
		{
			return default;
		}

		public ILifecycleEnumerable<ISiobhanPayload> GetObfuscatedValues(ILifecycleEnumerable<ISiobhanPayload> records)
		{
			return GetObfuscatedValues(records).ToLifecycleEnumerable();

			IEnumerable<ISiobhanPayload> GetObfuscatedValues(IEnumerable<ISiobhanPayload> records)
			{
				string fieldName;
				Type fieldType;
				object originalFieldValue, obfuscatedFieldValue;
				bool isFieldOptional = true;

				if ((object)records == null)
					throw new ArgumentNullException(nameof(records));

				foreach (ISiobhanPayload record in records)
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

							obfuscatedFieldValue = this.GetObfuscatedValue(field, originalFieldValue);
							obfuscatedSiobhanPayload.Add(fieldName, obfuscatedFieldValue);
						}
					}

					yield return obfuscatedSiobhanPayload;
				}
			}
		}

		public IAsyncLifecycleEnumerable<ISiobhanPayload> GetObfuscatedValuesAsync(IAsyncLifecycleEnumerable<ISiobhanPayload> asyncRecords, CancellationToken cancellationToken = default)
		{
			return null;
		}

		long IObfuscationContext.GetSignHash(object value)
		{
			long hash;

			hash = this.GetBoundedHash(DEFAULT_HASH_BUCKET_SIZE, value);

			return hash;
		}

		public ValueTask<long> GetSignHashAsync(object value, CancellationToken cancellationToken = default)
		{
			return default;
		}

		long IObfuscationContext.GetValueHash(long? size, object value)
		{
			long hash;

			hash = this.GetBoundedHash(size ?? DEFAULT_HASH_BUCKET_SIZE, value);

			return hash;
		}

		public ValueTask<long> GetValueHashAsync(long? size, object value, CancellationToken cancellationToken = default)
		{
			return default;
		}

		private object ResolveDictionaryValue(DictionaryConfiguration dictionaryConfiguration, object surrogateKey)
		{
			if ((object)dictionaryConfiguration == null)
				throw new ArgumentNullException(nameof(dictionaryConfiguration));

			if ((object)surrogateKey == null)
				throw new ArgumentNullException(nameof(surrogateKey));

			if ((object)this.ResolveDictionaryValueCallback == null)
				return null;

			return this.ResolveDictionaryValueCallback(dictionaryConfiguration, surrogateKey);
		}

		bool IObfuscationContext.TryGetSurrogateValue(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, out object surrogateValue)
		{
			IDictionary<object, object> dictionaryCache;

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

					surrogateValue = this.ResolveDictionaryValue(dictionaryConfiguration, surrogateKey);
					dictionaryCache.Add(surrogateKey, surrogateValue);
				}
			}
			else
			{
				surrogateValue = this.ResolveDictionaryValue(dictionaryConfiguration, surrogateKey);
			}

			return true;
		}

		public ValueTask<bool> TryGetSurrogateValueAsync(DictionaryConfiguration dictionaryConfiguration, object surrogateKey, out object surrogateValue, CancellationToken cancellationToken = default)
		{
			surrogateValue = null;
			return default;
		}

		#endregion
	}
}