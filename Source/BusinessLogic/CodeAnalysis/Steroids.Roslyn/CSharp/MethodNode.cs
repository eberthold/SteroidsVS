using System.Linq;
using Steroids.CodeStructure.Analyzers;
using Steroids.CodeStructure.Extensions;

namespace Steroids.Roslyn.CSharp
{
    public class MethodNode : CodeStructureItem
    {
        /// <inheritdoc />
        public override int OrderBaseValue => (int)SortOrder.Methods;
    }
}