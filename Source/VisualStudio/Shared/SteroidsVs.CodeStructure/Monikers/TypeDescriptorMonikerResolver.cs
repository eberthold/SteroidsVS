using System.Collections.Generic;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Steroids.CodeStructure.Analyzers;

namespace SteroidsVS.CodeStructure.Monikers
{
    /// <summary>
    /// Resolves a <see cref="ITypeDescriptor"/> to a <see cref="ImageMoniker"/>.
    /// </summary>
    internal class TypeDescriptorMonikerResolver
    {
        private static readonly Dictionary<string, ImageMoniker> _cache = new Dictionary<string, ImageMoniker>();
        private readonly TypeDescriptorMonikerNameResolver _nameResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescriptorMonikerResolver"/> class.
        /// </summary>
        public TypeDescriptorMonikerResolver()
        {
            _nameResolver = new TypeDescriptorMonikerNameResolver();
        }

        public ImageMoniker GetMoniker(ITypeDescriptor typeDescriptor)
        {
            var name = _nameResolver.Resolve(typeDescriptor);
            if (_cache.ContainsKey(name))
            {
                return _cache[name];
            }

            var moniker = typeof(KnownMonikers).GetProperty(name)?.GetValue(null) as ImageMoniker?;
            if (moniker is null)
            {
                return KnownMonikers.UnknownMember;
            }

            _cache.Add(name, moniker.Value);
            return moniker.Value;
        }
    }
}
