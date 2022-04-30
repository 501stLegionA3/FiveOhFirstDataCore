namespace ProjectDataCore.Data.Structures.Util.Comparers;
public class DuplicateComparer<TKey> : IComparer<TKey> where TKey : IComparable
{
    private readonly bool _sameIsLower;

    public DuplicateComparer(bool sameIsLower)
    {
        _sameIsLower = sameIsLower;
    }

    public int Compare(TKey? x, TKey? y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return _sameIsLower ? -1 : 1;
        return result;
    }
}
