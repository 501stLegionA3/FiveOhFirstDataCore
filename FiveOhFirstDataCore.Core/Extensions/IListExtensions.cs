using System.Linq.Expressions;

namespace FiveOhFirstDataCore.Data.Extensions
{
    public static class IListExtensions
    {
        public static HashSet<T> ToHashSet<TValue, T>(this IEnumerable<TValue> value, Expression<Func<TValue, T>> selector) where TValue : class
        {
            var set = new HashSet<T>();
            var func = selector.Compile();
            foreach (var item in value)
            {
                set.Add(func.Invoke(item));
            }
            return set;
        }

        public static List<T> ToList<TValue, T>(this IEnumerable<TValue> value, Expression<Func<TValue, T>> selector) where TValue : class
        {
            var set = new List<T>();
            var func = selector.Compile();
            foreach (var item in value)
            {
                set.Add(func.Invoke(item));
            }
            return set;
        }
    }
}
