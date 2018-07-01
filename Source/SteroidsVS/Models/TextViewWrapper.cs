using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.Core.Diagnostics.Contracts;
using Steroids.Core.Extensions;

namespace SteroidsVS.Models
{
    /// <summary>
    /// Wrapper for the <see cref="IWpfTextView"/> to improve testability.
    /// Mostly because some important extension methods cannot be mocked in a nice manner.
    /// </summary>
    internal class TextViewWrapper : IQualityTextView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextViewWrapper"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        public TextViewWrapper(IWpfTextView textView)
        {
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
        }

        /// <inheritdoc />
        public string Path
        {
            get
            {
                if (TextView.TextBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out var document) && document != null)
                {
                    return document.FilePath;
                }

                return TextView?.GetDocument()?.FilePath ?? string.Empty;
            }
        }

        /// <inheritdoc />
        public IWpfTextView TextView { get; }
    }
}
