using System;
using System.Text;

namespace MsGraph.Classes
{
    public static class GraphQuery
    {

        
        public static string Combine(params string[] parts)
        {
            if (parts.Length == 0) throw new ArgumentException("No parts");
            var sb = new StringBuilder();
            for (var i = 0; i < parts.Length; i++)
            {
                var s = parts[i];
                if (s == null) continue;
                if (s.Length == 0) continue;
                if (s[0] == '?' || s[0] == '&') s = s.Remove(0, 1);
                sb.Append(i == 0 ? "?" : "&");
                sb.Append(s);
            }

            return sb.ToString();
        }
    }
}
