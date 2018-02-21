using System.Windows;
using System.Windows.Controls;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.CodeStructure.TemplateSelectors
{
    public class NodeContainerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SectionHeaderTemplate { get; set; }

        public DataTemplate NodeContainerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ICodeStructureSectionHeader)
            {
                return SectionHeaderTemplate;
            }

            if (item is ICodeStructureNodeContainer)
            {
                return NodeContainerTemplate;
            }

            return null;
        }
    }
}
