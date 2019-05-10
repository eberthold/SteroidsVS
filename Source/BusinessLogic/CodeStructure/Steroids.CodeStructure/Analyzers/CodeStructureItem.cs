using System;
using Steroids.Core;

namespace Steroids.CodeStructure.Analyzers
{
    /// <summary>
    /// Represents a element of code like a method or variable.
    /// </summary>
    public class CodeStructureItem : BindableBase, IComparable<CodeStructureItem>
    {
        private string _accessModifier = string.Empty;
        private string _parameters;
        private string _name;
        private string _returnType;
        private int _startLineNumber;
        private int _endLineNumber;
        private bool _isMeta;
        private int _orderBaseValue;
        private ITypeDescriptor _imageDescriptor;

        /// <summary>
        /// The access modifier of this item.
        /// </summary>
        public string AccessModifier { get => _accessModifier; set => Set(ref _accessModifier, value); }

        /// <summary>
        /// The parameters for this item.
        /// </summary>
        public string Parameters { get => _parameters; set => Set(ref _parameters, value); }

        /// <summary>
        /// The name of this item.
        /// </summary>
        public string Name { get => _name; set => Set(ref _name, value); }

        /// <summary>
        /// The return type or data type of this item.
        /// </summary>
        public string ReturnType { get => _returnType; set => Set(ref _returnType, value); }

        /// <summary>
        /// The line number on which this item block begins.
        /// </summary>
        public int StartLineNumber { get => _startLineNumber; set => Set(ref _startLineNumber, value); }

        /// <summary>
        /// The line number on which this item block ends.
        /// </summary>
        public int EndLineNumber { get => _endLineNumber; set => Set(ref _endLineNumber, value); }

        /// <summary>
        /// <see langword="true"/> if the node is just a meta node for grouping items logically.
        /// </summary>
        public virtual bool IsMeta { get => _isMeta; set => Set(ref _isMeta, value); }

        /// <summary>
        /// The order in which this item type belongs to other item types.
        /// </summary>
        public virtual int OrderBaseValue { get => _orderBaseValue; set => Set(ref _orderBaseValue, value); }

        /// <summary>
        /// The descriptor to specifically identify this type of item e.g. to specify a image for UI.
        /// </summary>
        public ITypeDescriptor TypeDescriptor { get => _imageDescriptor; set => Set(ref _imageDescriptor,  value); }

        /// <inheritdoc />
        public int CompareTo(CodeStructureItem other)
        {
            var order = OrderBaseValue.CompareTo(other.OrderBaseValue);
            if (order != 0)
            {
                return order;
            }

            order = Name.CompareTo(other.Name);
            return order;
        }

        /// <inheritdoc />
        public override string ToString() => Name;
    }
}
