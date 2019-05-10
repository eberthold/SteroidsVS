using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.CSharp;

namespace SteroidsVS.CodeStructure.Monikers
{
    internal class TypeDescriptorMonikerNameResolver
    {
        /// <summary>
        /// Resolves the name of the moniker name of the given type descriptor.
        /// </summary>
        /// <param name="typeDescriptor">The <see cref="ITypeDescriptor"/>.</param>
        /// <returns>The complete name.</returns>
        internal string Resolve(ITypeDescriptor typeDescriptor)
        {
            switch (typeDescriptor)
            {
                case CSharpTypeDescriptor csharp:
                    return csharp.TypeName + csharp.AccessModifier;
                default:
                    return typeDescriptor.TypeName;
            }
        }
    }
}
