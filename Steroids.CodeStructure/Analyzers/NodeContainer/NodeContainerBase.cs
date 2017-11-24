namespace Steroids.CodeStructure.Analyzers.NodeContainer
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Imaging.Interop;
    using Steroids.Common;

    public abstract class NodeContainerBase<T> : BindableBase, ICodeStructureNodeContainer
        where T : CSharpSyntaxNode
    {
        private string _accessModifier = null;
        private string _id = null;
        private int _indentLevel = -1;
        private ImageMoniker _moniker = KnownMonikers.UnknownMember;
        private string _name = null;
        private T _node;
        private string _parameters = null;
        private ICodeStructureNodeContainer _parent;
        private ICommand _scrollToNodeCommand;
        private string _type = null;

        /// <summary>
        /// Gets or sets the <see cref="ICodeStructureNodeContainer.AbsoluteIndex"/>.
        /// </summary>
        public int AbsoluteIndex { get; set; }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.AccessModifier"/>.
        /// </summary>
        public string AccessModifier
        {
            get
            {
                if (_accessModifier == null)
                {
                    AccessModifier = GetAccessModifier();
                }

                return _accessModifier;
            }

            private set
            {
                if (Set(ref _accessModifier, value))
                {
                    Moniker = KnownMonikers.UnknownMember;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICodeStructureNodeContainer.AnalyzeId"/>.
        /// </summary>
        public Guid AnalyzeId { get; set; }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.Id"/>.
        /// Converts a combination of all infos that make a node unique to a int hash.
        /// </summary>
        public string Id
        {
            get
            {
                _id = _id ?? $"{GetType().Name}{Name}{Parameters}";
                return _id;
            }

            private set
            {
                Set(ref _id, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.IndentLevel"/>.
        /// </summary>
        public int IndentLevel
        {
            get
            {
                if (_indentLevel == -1)
                {
                    _indentLevel = Parent?.IndentLevel + 1 ?? 0;
                }

                return _indentLevel;
            }

            private set
            {
                Set(ref _indentLevel, value);
            }
        }

        public virtual int LastAbsoluteIndex => AbsoluteIndex;

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.Moniker"/>
        /// </summary>
        public ImageMoniker Moniker
        {
            get
            {
                if (_moniker.Equals(KnownMonikers.UnknownMember))
                {
                    Moniker = GetMoniker();
                }

                return _moniker;
            }

            private set
            {
                Set(ref _moniker, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.Name"/>
        /// </summary>
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    Name = GetName();
                }

                return _name;
            }

            private set
            {
                if (Set(ref _name, value))
                {
                    Id = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        public T Node
        {
            get
            {
                return _node;
            }

            set
            {
                if (!Set(ref _node, value))
                {
                    return;
                }

                AccessModifier = GetAccessModifier();
                Type = GetReturnType();
                Name = GetName();
                Parameters = GetParameters();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICodeStructureNodeContainer.Node"/>.
        /// </summary>
        SyntaxNode ICodeStructureNodeContainer.Node
        {
            get
            {
                return _node;
            }

            set
            {
                Set(ref _node, value as T);
            }
        }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.Parameters"/>.
        /// </summary>
        public string Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    Parameters = GetParameters();
                }

                return _parameters;
            }

            private set
            {
                if (Set(ref _parameters, value))
                {
                    Id = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICodeStructureNodeContainer.Parent"/>.
        /// </summary>
        public ICodeStructureNodeContainer Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (Set(ref _parent, value))
                {
                    IndentLevel = -1;
                }
            }
        }

        public ICommand ScrollToNodeCommand
        {
            get { return _scrollToNodeCommand; }
            set { Set(ref _scrollToNodeCommand, value); }
        }

        /// <summary>
        /// Gets the <see cref="ICodeStructureNodeContainer.Type"/>
        /// </summary>
        public string Type
        {
            get
            {
                if (_type == null)
                {
                    Name = GetReturnType();
                }

                return _type;
            }

            private set
            {
                Set(ref _type, value);
            }
        }

        /// <summary>
        /// Gets the access modifier of the node.
        /// </summary>
        /// <returns>The access modifier.</returns>
        protected abstract string GetAccessModifier();

        /// <summary>
        /// Gets the <see cref="ImageMoniker"/> which fits for current node.
        /// </summary>
        /// <returns>The <see cref="ImageMoniker"/>.</returns>
        protected abstract ImageMoniker GetMoniker();

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        /// <returns>The name.</returns>
        protected abstract string GetName();

        /// <summary>
        /// Gets the parameters of the node.
        /// </summary>
        /// <returns>The parameters.</returns>
        protected abstract string GetParameters();

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <returns>The type.</returns>
        protected abstract string GetReturnType();
    }
}