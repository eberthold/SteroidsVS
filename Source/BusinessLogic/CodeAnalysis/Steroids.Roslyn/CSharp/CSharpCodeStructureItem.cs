using System.Linq;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.CSharp
{
    /// <summary>
    /// Implementation to automatically update <see cref="CodeStructureItem.TypeDescriptor"/> when 
    /// <see cref="CodeStructureItem.AccessModifier"/> changes.
    /// </summary>
    public abstract class CSharpCodeStructureItem : CodeStructureItem
    {
        public CSharpCodeStructureItem()
        {
            this.PropertyChanged += OnPropertyChanged;
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
            TypeDescriptor = CSharpTypeDescriptor
                .KnownCombinations
                .FirstOrDefault(x =>
                    x.TypeName == NodeTypeName &&
                    x.AccessModifier == AccessModifier);
        }
    }
}
