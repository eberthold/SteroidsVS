using System;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.Contracts.UI;
using Steroids.Core.Extensions;

namespace Steroids.CodeQuality.UI
{
    /// <summary>
    /// Provides the logic to place a <see cref="DiagnosticInfoLine"/> element.
    /// </summary>
    public static class DiagnosticInfoPlacementCalculator
    {
        /// <summary>
        /// Calculates the placement <see cref="Rect"/> of the <paramref name="diagnosticInfoLine"/> ind the <paramref name="textView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> in which the line should be placed.</param>
        /// <param name="diagnosticInfoLine">The <see cref="DiagnosticInfoLine"/> to place.</param>
        /// <param name="adornmentSpaceReservation">The <see cref="IAdornmentSpaceReservation"/>.</param>
        /// <returns>The placement <see cref="Rect"/>.</returns>
        public static Rect CalculatePlacementRect(IWpfTextView textView, DiagnosticInfoLine diagnosticInfoLine, IAdornmentSpaceReservation adornmentSpaceReservation)
        {
            var lineSnapshot = textView.GetSnapshotForLineNumber(diagnosticInfoLine.LineNumber);
            if (lineSnapshot == null)
            {
                return Rect.Empty;
            }

            var snapshotSpan = lineSnapshot.Extent;
            var endPoint = snapshotSpan.End;

            try
            {
                if (!textView.TextViewLines.ContainsBufferPosition(endPoint))
                {
                    return Rect.Empty;
                }
            }
            catch (InvalidCastException)
            {
                // Don't know exactly why this can happen, but it occasionally occurs since performance improvements
                // in  parsing diagnostic infos were done.
                return Rect.Empty;
            }

            var textViewLine = textView.GetTextViewLineContainingBufferPosition(endPoint);

            var isVisible = textViewLine.VisibilityState > VisibilityState.PartiallyVisible;
            if (!isVisible)
            {
                return Rect.Empty;
            }

            var reservedWidth = adornmentSpaceReservation?.ActualWidth ?? 0.0;

            var height = textViewLine.TextBottom - textViewLine.TextTop;
            var left = Math.Min(textViewLine.TextRight + 10, textView.ViewportRight - height - reservedWidth) - textView.ViewportLeft;
            var width = Math.Max(textView.ViewportWidth - left - reservedWidth, height);
            var top = textViewLine.Top - textView.ViewportTop + ((textViewLine.Bottom - textViewLine.Top - height) / 2);

            return new Rect(left, top, width, height);
        }

        /// <summary>
        /// Maps the line number to the real line number as it's currently displayed in the text view.
        /// So if a region is collapsed, all diagnostics in this region should be mapped to the same line.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="lineNumber">The number of the line.</param>
        /// <param name="outliningManager">The <see cref="IOutliningManager"/>.</param>
        /// <returns>The real line number.</returns>
        public static int GetRealLineNumber(IWpfTextView textView, int lineNumber, IOutliningManager outliningManager)
        {
            try
            {
                // a line could not get any further than 0 even by collapsing regions
                if (lineNumber == 0)
                {
                    return 0;
                }

                // if no snapshot line found return 0
                var lineSnapshot = textView.GetSnapshotForLineNumber(lineNumber);
                if (lineSnapshot == null)
                {
                    return 0;
                }

                // if no collapsed region than line number fits as normal
                var snapshotSpan = lineSnapshot.Extent;
                var region = outliningManager?.GetCollapsedRegions(snapshotSpan) ?? Enumerable.Empty<ICollapsible>();
                if (!region.Any())
                {
                    return lineNumber;
                }

                // I assume that the longest collapsed region is the outermost
                var regionSnapshot = region
                    .Select(x => x.Extent.GetSpan(textView.TextSnapshot))
                    .ToDictionary(x => x.Length)
                    .OrderByDescending(x => x.Key)
                    .First()
                    .Value;

                var collapsedLineNumber = textView.TextSnapshot.GetLineNumberFromPosition(regionSnapshot.End.Position);
                return collapsedLineNumber;
            }
            catch (ObjectDisposedException ex)
            {
                if (ex.ObjectName == "OutliningMnger")
                {
                    // TODO: when we have a logger service add logging
                }

                // I assume that this case seems to happen, if the TextView gets closed and we receive a
                // DiagnosticChanged event right in the time frame before we dispose the whole container graph.
                return lineNumber;
            }
        }
    }
}
