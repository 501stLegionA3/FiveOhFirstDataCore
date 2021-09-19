namespace FiveOhFirstDataCore.Data.Extensions
{
    public static class ObjectArrayExtensions
    {
        public static T? GetArgument<T>(this object[] args, int pos)
            => (T)Convert.ChangeType(args[pos], typeof(T));
    }
}
