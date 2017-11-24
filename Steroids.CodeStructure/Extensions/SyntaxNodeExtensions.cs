namespace Steroids.CodeStructure.Extensions
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class SyntaxNodeExtensions
    {
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
