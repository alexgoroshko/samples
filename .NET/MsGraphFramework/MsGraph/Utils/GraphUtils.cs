namespace MsGraph.Utils
{
    public static class GraphUtils
    {
        public static bool IsBlank(this string @this)
        {
            return string.IsNullOrWhiteSpace(@this);
        }

    }
}
