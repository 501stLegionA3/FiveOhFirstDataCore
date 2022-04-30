namespace ProjectDataCore.Data.Structures.Util.Comparers;
public class DuplicateComparer<TKey> : IComparer<TKey> where TKey : IComparable
{
    public int Compare(TKey? x, TKey? y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1;
        return result;
    }
}
