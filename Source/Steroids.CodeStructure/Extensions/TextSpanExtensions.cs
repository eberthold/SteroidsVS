using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace Steroids.CodeStructure.Extensions
{
    public static class TextSpanExtensions
    {
        /// <summary>
        /// Convert a <see cref="TextSpan"/> instance to an <see cref="TextSpan"/>.
        /// </summary>
        /// <param name="textSpan">The <see cref="TextSpan"/>.</param>
        /// <returns>The <see cref="TextSpan"/> as <see cref="Span"/>.</returns>
        public static Span ToSpan(this TextSpan textSpan)
        {
            return new Span(textSpan.Start, textSpan.Length);
        }

        /// <summary>
        /// Convert a <see cref="TextSpan"/> to a <see cref="SnapshotSpan"/> on the given <see cref="ITextSnapshot"/> instance.
        /// </summary>
        /// <param name="textSpan">The <see cref="TextSpan"/>.</param>
        /// <param name="snapshot">The <see cref="ITextSnapshot"/>.</param>
        /// <returns>The <see cref="SnapshotSpan"/>.</returns>
        public static SnapshotSpan ToSnapshotSpan(this TextSpan textSpan, ITextSnapshot snapshot)
        {
            var span = textSpan.ToSpan();
            return new SnapshotSpan(snapshot, span);
        }
    }
}
