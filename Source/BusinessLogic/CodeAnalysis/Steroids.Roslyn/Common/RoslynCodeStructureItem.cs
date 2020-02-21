using System.Linq;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.Common
{
    /// <summary>
    /// Implementation to automatically update <see cref="CodeStructure.Analyzers.CodeStructureItem.TypeDescriptor"/> when
    /// <see cref="CodeStructure.Analyzers.CodeStructureItem.AccessModifier"/> changes.
    /// </summary>
    public abstract class RoslynCodeStructureItem : CodeStructure.Analyzers.CodeStructureItem
    {
        public RoslynCodeStructureItem()
        {
            PropertyChanged += OnPropertyChanged;
            UpdateTypeDescriptor();
        }

        /// <summary>
        /// Gets the type name of the node.
        /// </summary>
        protected abstract string NodeTypeName { get; }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccessModifier))
            {
                UpdateTypeDescriptor();
            }
        }

        private void UpdateTypeDescriptor()
        {
            TypeDescriptor = RoslynTypeDescriptor
                .KnownCombinations
                .FirstOrDefault(x =>
                    x.TypeName == NodeTypeName &&
                    x.AccessModifier == AccessModifier);
        }
    }
}
