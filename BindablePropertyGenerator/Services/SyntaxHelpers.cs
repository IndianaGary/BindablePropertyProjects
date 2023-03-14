namespace BindablePropertyGenerator;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class SyntaxHelpers
{
    /// <summary>
    /// Returns a simple name regardless of whether or not it is qualified,
    /// or an empty string if neither
    /// </summary>
    public static string ExtractName( NameSyntax? name ) 
        => name switch
        {
            SimpleNameSyntax    simple      => simple.Identifier.Text,
            QualifiedNameSyntax qualified   => qualified.Right.Identifier.Text,
            _ => ""
        };

    /// <summary>
    /// Validate attribute name with or without qualification
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool IsValidAttribute( NameSyntax? name )
    {
        if ( name is SimpleNameSyntax simple && 
             simple.Identifier.Text is "BindableProperty" or "BindablePropertyAttribute" )
            return true;

        return name is QualifiedNameSyntax qualified &&
             qualified.Right.Identifier.Text is "BindableProperty" or "BindablePropertyAttribute" &&
             qualified.Left is SimpleNameSyntax nameSpace &&
             nameSpace.Identifier.Text is "BindablePropertyAttributes";
    }

    /// <summary>
    /// Used by the generator to determine whether the specified field is valid.
    /// </summary>
    public static bool IsValidFieldSymbol( ISymbol symbol )
    {
        if ( symbol is not IFieldSymbol fieldSymbol )
            return false;

        var attributes  =   fieldSymbol.GetAttributes();

        var result      =   attributes.Any( attr => ( attr.AttributeClass?.Name == "BindableProperty" ||
                                                      attr.AttributeClass?.Name == "BindablePropertyAttribute" ) &&
                                                      attr.AttributeClass.ContainingNamespace is
                                                      {
                                                          Name: "BindablePropertyAttributes",
                                                          ContainingNamespace.IsGlobalNamespace: true
                                                      });
        return result;
    }

    public static bool TryGetDeclaringClass( FieldDeclarationSyntax fieldDeclaration, out ClassDeclarationSyntax? classDeclaration )
    {
        var parentNode      =   fieldDeclaration.Parent;

        //  Find declaring class
        while ( parentNode is not null and not ClassDeclarationSyntax )
            parentNode = parentNode.Parent;
                
        if ( parentNode is null )
        {
            classDeclaration = null;
            return false;
        }

        classDeclaration = (ClassDeclarationSyntax)parentNode;
        return true;
    }

    /// <summary>
    /// Build up the fully qualified class name by prefixing the namespace name(s)
    /// </summary>
    public static string GetFullyQualifiedNamespaceName( TypeDeclarationSyntax typeDeclaration, out bool isFileScoped )
    {
        //  Extract class name
        isFileScoped   = false;

        //  Look for the nearest namespace declaration by walking the syntax tree
        var parentNode = typeDeclaration.Parent;

        while ( parentNode is not null and
                not NamespaceDeclarationSyntax and
                not FileScopedNamespaceDeclarationSyntax )
            parentNode = parentNode.Parent;

        if ( parentNode is FileScopedNamespaceDeclarationSyntax fileScopedNamespace )
        {
            isFileScoped = true;
            return fileScopedNamespace.Name.ToString();
        }

        //  Default to global/RootNamespace
        var namespaceName = string.Empty;

        if ( parentNode is NamespaceDeclarationSyntax namespaceDeclaration )
        {
            namespaceName = namespaceDeclaration.Name.ToString();

            //  Inline namespaces can be nested; walk up the syntax tree
            while ( namespaceDeclaration.Parent is NamespaceDeclarationSyntax namespaceParent )
            {
                //  Prefix the inner namespace with the next outer namespace
                namespaceDeclaration = namespaceParent;
                namespaceName        = namespaceDeclaration.Name.ToString() + "." + namespaceName;
            }
        }

        return namespaceName;
    }
}
