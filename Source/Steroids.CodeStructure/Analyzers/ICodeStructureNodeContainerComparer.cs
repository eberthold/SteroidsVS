using System.Collections.Generic;

namespace Steroids.CodeStructure.Analyzers
{
    public class ICodeStructureNodeContainerComparer : IComparer<ICodeStructureNodeContainer>
    {
        /// <inheritdoc />
        public int Compare(ICodeStructureNodeContainer x, ICodeStructureNodeContainer y)
        {
            return x.AbsoluteIndex - y.AbsoluteIndex;
        }
    }
}
