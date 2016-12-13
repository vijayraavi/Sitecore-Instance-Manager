namespace SIM.Extensions
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  public static class DictionaryExtensions
  {
    public static TValue TryGetValue<TKey, TValue>([NotNull] this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
    {
      Assert.ArgumentNotNull(dictionary, nameof(dictionary));

      TValue value;
      return dictionary.TryGetValue(key, out value) ? value : null;
    }

    public static TValue GetOrCreate<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> func) where TValue : class
    {
      Assert.ArgumentNotNull(dictionary, nameof(dictionary));

      TValue value;
      if (dictionary.TryGetValue(key, out value))
      {
        return value;
      }

      value = func();
      Assert.IsNotNull(value, nameof(value));

      dictionary.Add(key, value);

      return value;
    }
                               
    [NotNull]
    public static IReadOnlyDictionary<TKeyInner, TValue> GetOrCreate<TKey, TKeyInner, TValue>([NotNull] this IDictionary<TKey, IReadOnlyDictionary<TKeyInner, TValue>> dictionary, TKey key) 
    {
      return GetOrCreate(dictionary, key, () => new Dictionary<TKeyInner, TValue>()).IsNotNull();
    }
             
    [NotNull]
    public static IDictionary<TKeyInner, TValue> GetOrCreate<TKey, TKeyInner, TValue>([NotNull] this IDictionary<TKey, IDictionary<TKeyInner, TValue>> dictionary, TKey key) 
    {
      return GetOrCreate(dictionary, key, () => new Dictionary<TKeyInner, TValue>()).IsNotNull();
    }

    [NotNull]
    public static IDictionary<TKeyInner, TValue> GetOrCreate<TKey, TKeyInner, TValue>([NotNull] this IDictionary<TKey, Dictionary<TKeyInner, TValue>> dictionary, TKey key) 
    {
      return GetOrCreate(dictionary, key, () => new Dictionary<TKeyInner, TValue>()).IsNotNull();
    }
  }
}
