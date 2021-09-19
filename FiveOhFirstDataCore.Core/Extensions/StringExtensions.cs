namespace FiveOhFirstDataCore.Data.Extensions
{
    public static class StringExtensions
    {
        public static bool IsAlphabetical(this string value)
        {
            int nums = 0;
            foreach (var c in value)
            {
                if (nums > 0) return false;

                nums += c switch
                {
                    >= 'a' and <= 'z' => 0,
                    >= 'A' and <= 'Z' => 0,
                    _ => 1
                };
            }

            return nums == 0;
        }
    }
}
