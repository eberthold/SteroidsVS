using System;
using Steroids.Core;

namespace Steroids.CodeStructure.Analyzers
{
    public class CodeStructureItem : BindableBase, ICodeStructureItem
    {
        private string _accessModifier;
        private string _parameters;
        private string _name;
        private string _returnType;
        private Guid _analyzeId;
        private string _nodeType;
        private int _startLineNumber;
        private int _endLineNumber;
        private bool _isMeta;
        private int _orderBaseValue;

        /// <inheritdoc/>
        public string AccessModifier { get => _accessModifier; set => Set(ref _accessModifier, value); }

        /// <inheritdoc/>
        public string Parameters { get => _parameters; set => Set(ref _parameters, value); }

        /// <inheritdoc/>
        public string Name { get => _name; set => Set(ref _name, value); }

        /// <inheritdoc/>
        public string ReturnType { get => _returnType; set => Set(ref _returnType, value); }

        /// <inheritdoc/>
        public Guid AnalyzeId { get => _analyzeId; set => Set(ref _analyzeId, value); }

        /// <inheritdoc/>
        public string NodeType { get => _nodeType; set => Set(ref _nodeType, value); }

        /// <inheritdoc/>
        public int StartLineNumber { get => _startLineNumber; set => Set(ref _startLineNumber, value); }

        /// <inheritdoc/>
        public int EndLineNumber { get => _endLineNumber; set => Set(ref _endLineNumber, value); }

        /// <inheritdoc />
        public virtual bool IsMeta { get => _isMeta; set => Set(ref _isMeta, value); }

        /// <inheritdoc />
        public virtual int OrderBaseValue { get => _orderBaseValue; set => Set(ref _orderBaseValue, value); }

        /// <inheritdoc />
        public int CompareTo(ICodeStructureItem other)
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
