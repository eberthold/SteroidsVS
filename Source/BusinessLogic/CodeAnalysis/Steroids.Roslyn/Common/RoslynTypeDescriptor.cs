using System.Collections.Generic;
using System.Linq;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.Common
{
    public class RoslynTypeDescriptor : ITypeDescriptor
    {
        private static readonly IReadOnlyCollection<string> KnownAccessModifiers = new List<string>
        {
            "Private",
            "Protected",
            "Internal",
            "Public",
            string.Empty
        };

        private static readonly IReadOnlyCollection<string> KnownTypes = new List<string>
        {
            "Class",
            "Constructor",
            "Enumeration",
            "EnumerationItem",
            "Event",
            "Field",
            "Interface",
            "Method",
            "Property",
            "Struct"
        };

        private RoslynTypeDescriptor()
        {
        }

        /// <summary>
        /// Gets the access modifier to specify the type better.
        /// </summary>
        public string AccessModifier { get; private set; }

        /// <inheritdoc />
        public string TypeName { get; private set; }

        /// <summary>
        /// Contains a list with all combinations of types and access modifiers.
        /// </summary>
        public static IReadOnlyCollection<RoslynTypeDescriptor> KnownCombinations { get; } = CreateKnownCombinations();

        /// <summary>
        /// Creates the combinations of type and access modifiers.
        /// </summary>
        private static IReadOnlyCollection<RoslynTypeDescriptor> CreateKnownCombinations()
        {
            var result = new List<RoslynTypeDescriptor>(KnownTypes.Count * KnownAccessModifiers.Count);

            for (int i = 0; i < KnownTypes.Count; i++)
            {
                for (int j = 0; j < KnownAccessModifiers.Count; j++)
                {
                    result.Add(new RoslynTypeDescriptor
                    {
                        TypeName = KnownTypes.ElementAt(i),
                        AccessModifier = KnownAccessModifiers.ElementAt(j)
                    });
                }
            }

            return result;
        }
    }
}
