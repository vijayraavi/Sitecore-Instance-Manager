namespace SIM.Extensions
{
  using System;
  using System.Linq;
  using JetBrains.Annotations;

  public static class Extensions
  {
    [NotNull]
    public static T IsNotNull<T>(this T source, string message = null) where T : class
    {
      Assert.IsNotNull(source, message ?? "Value is null");

      return source;
    }

    public static bool Equals<T, TR>([NotNull] this T that, object obj, [NotNull] params Func<T, TR>[] comparers) where T : class where TR : IEquatable<TR>
    {
      Assert.ArgumentNotNull(that, nameof(that));

      if (ReferenceEquals(that, obj))
      {
        return true;
      }

      var other = obj as T;
      if (other == null)
      {
        return false;
      }

      // ReSharper disable once PossibleNullReferenceException
      return comparers.All(x => x.Invoke(that).Equals(x(other)));
    }

    public static TResult With<TInput, TResult>(this TInput @this, Func<TInput, TResult> evaluator)
      where TResult : class
      where TInput : class
    {
      if (@this == null)
      {
        return null;
      }

      return evaluator(@this);
    }           
  }
}