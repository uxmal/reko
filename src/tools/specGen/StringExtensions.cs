using System.IO;

namespace Reko.Tools.specGen
{
    internal static class StringExtensions
    {
        public static string NormalizePath(this string path)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            path = path.Replace('/', Path.DirectorySeparatorChar);
            return path;
        }

        public static string TrimHeadingSlash(this string path)
        {
            return path.TrimStart('/', '\\');
        }

        public static string TrimTrailingSlash(this string path)
        {
            return path.TrimEnd('/', '\\');
        }

        public static string TrimSlashes(this string path){
            return path.TrimHeadingSlash().TrimTrailingSlash();
        }

        public static string EnsureTrailingSlash(this string path)
        {
            if(!path.EndsWith(Path.DirectorySeparatorChar))
            {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }
    }
}
