using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steroids.Roslyn.CSharp
{
    public static class KnownImages
    {
        public const string Field = "Field";
        public const string Constructor = "Method";
        public const string Enum = "EnumerationItem";
        public const string Event = "Event";
        public const string Property = "Property";
        public const string Method = "Method";
        public const string Class = "Constructor";
        public const string Struct = "Constructor";

        public static string GetImageName(string type, string visibility)
        {
            return $"{type}{visibility}";
        }
    }
}
