using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.CSharp;
using Steroids.Roslyn.Resources.Strings;

namespace Steroids.Roslyn.StructureAnalysis
{
    public static class CSharpMetaNodeCreator
    {
        /// <summary>
        /// Creates a meta node for the given item.
        /// </summary>
        /// <param name="member">The member which needs a meta node to be grouped in.</param>
        /// <returns>The newly created meta node.</returns>
        public static CodeStructureItem Create(CodeStructureItem member)
        {
            switch (member)
            {
                case FieldNode _:
                    return new FieldNode
                    {
                        IsMeta = true,
                        Name = Strings.FieldMetaNode_Name
                    };

                case ConstructorNode _:
                    return new ConstructorNode
                    {
                        IsMeta = true,
                        Name = Strings.ConstructorMetaNode_Name
                    };

                case EventNode _:
                    return new EventNode
                    {
                        IsMeta = true,
                        Name = Strings.EventMetaNode_Name
                    };

                case PropertyNode _:
                    return new PropertyNode
                    {
                        IsMeta = true,
                        Name = Strings.PropertyMetaNode_Name
                    };

                case MethodNode _:
                    return new MethodNode
                    {
                        IsMeta = true,
                        Name = Strings.MethodMetaNode_Name
                    };

                case EnumNode _:
                    return new EnumNode
                    {
                        IsMeta = true,
                        Name = member.Name
                    };

                default:
                    return new CodeStructureItem
                    {
                        IsMeta = true,
                        Name = member.GetType().Name
                    };
            }
        }
    }
}
