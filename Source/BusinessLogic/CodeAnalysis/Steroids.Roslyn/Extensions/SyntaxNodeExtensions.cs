using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Steroids.Roslyn.Extensions
{
    public static class SyntaxNodeExtensions
    {
        /// <summary>
        /// Extracts the access modifier information from a syntax node.
        /// </summary>
        /// <param name="list">The list of syntax tokens.</param>
        /// <returns>The access modifier string, fall back value is private.</returns>
        public static string GetAccessModifier(this SyntaxTokenList list)
        {
            if (list.Any(x => x.IsKind(SyntaxKind.PublicKeyword)))
            {
                return "Public";
            }

            if (list.Any(x => x.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return "Private";
            }

            if (list.Any(x => x.IsKind(SyntaxKind.InternalKeyword)))
            {
                return "Internal";
            }

            if (list.Any(x => x.IsKind(SyntaxKind.ProtectedKeyword)))
            {
                return "Protected";
            }

            if (list.Any(x => x.IsKind(SyntaxKind.SealedKeyword)))
            {
                return "Sealed";
            }

            return "Private";
        }
    }
}
