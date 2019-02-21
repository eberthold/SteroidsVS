using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Steroids.CodeStructure.Analyzers
{
    /// <summary>
    /// A tree, which enumerates from the root to the deepest level fast and then slowly up again by visiting all leaves first.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Data"/>.</typeparam>
    public class SortedTree<T> : IReadOnlyCollection<SortedTree<T>>
        where T : IComparable<T>
    {
        private readonly List<SortedTree<T>> _children = new List<SortedTree<T>>();

        public SortedTree(T data)
        {
            Data = data;
        }

        public SortedTree(T data, object meta)
        {
            Data = data;
            Meta = meta;
        }

        /// <summary>
        /// The data of this node.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Meta data of this node.
        /// </summary>
        public object Meta { get; }

        /// <summary>
        /// <see langword="true"/> if this is the root of the tree.
        /// </summary>
        public bool IsRoot => Parent is null;

        /// <summary>
        /// <see langword="true"/> if this node has no childs.
        /// </summary>
        public bool IsLeaf => Children.Count == 0;

        /// <summary>
        /// The depth in which this node sits relative to the tree root.
        /// </summary>
        public int Level => IsRoot ? 0 : Parent.Level + 1;

        /// <summary>
        /// The direct parent of this node.
        /// </summary>
        public SortedTree<T> Parent { get; private set; }

        /// <summary>
        /// All direct children of this node.
        /// </summary>
        public IReadOnlyList<SortedTree<T>> Children => _children;

        /// <inheritdoc />
        public int Count => Children.Count + Children.Sum(x => x.Count) + (IsRoot ? 1 : 0);

        /// <summary>
        /// Adds a new child node.
        /// </summary>
        /// <param name="child">The child data.</param>
        /// <returns>The new child as tree.</returns>
        public SortedTree<T> Add(T child)
        {
            var node = new SortedTree<T>(child);
            return Add(node);
        }

        /// <summary>
        /// Adds a new child node.
        /// </summary>
        /// <param name="child">The child tree to add.</param>
        /// <returns>The complete tree.</returns>
        public SortedTree<T> Add(SortedTree<T> child)
        {
            _children.Add(child);
            child.Parent = this;
            return child;
        }

        /// <inheritdoc />
        public IEnumerator<SortedTree<T>> GetEnumerator()
        {
            foreach (var child in DeepQueryTree())
            {
                yield return child;
            }
        }

        private IEnumerable<SortedTree<T>> DeepQueryTree()
        {
            return new[] { this }.Concat(Children.OrderBy(x => x.Data).SelectMany(x => x.DeepQueryTree()));
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
