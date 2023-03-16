namespace BindablePropertyServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class SyntaxExtensions
{
    /// <summary>
    /// Determines whether a class declaration includes the partial modifier
    /// </summary>
    public static bool IsPartialClass( this ClassDeclarationSyntax cls )
    {
        var result = cls.Modifiers.Any( modifier => modifier.IsKind( SyntaxKind.PartialKeyword ) );
        return result;
    }

    /// <summary>
    /// Determines whether a class declaration includes the static modifier
    /// </summary>
    public static bool IsStaticClass( this ClassDeclarationSyntax cls )
    {
        var result = cls.Modifiers.Any( modifier => modifier.IsKind( SyntaxKind.StaticKeyword ) );
        return result;
    }

    public static bool IsValidClassDeclaration( this ClassDeclarationSyntax cls ) 
            => ! cls.IsStaticClass() && cls.IsPartialClass();
}
