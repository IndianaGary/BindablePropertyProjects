namespace BindablePropertyServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;

public static class Helpers
{
    public static ImmutableHashSet<string> ValidAttributeArguments
            => ImmutableHashSet.Create(
                                        "DeclaringType",
                                        "DefaultBindingMode",
                                        "DefaultValue",
                                        "ValidateValue",
                                        "PropertyChanged",
                                        "PropertyChanging",
                                        "CoerceValue",
                                        "DefaultValueCreator",
                                        "HidesBaseProperty"
                                      );

    public static ImmutableHashSet<string> ValidBindingModes
            => ImmutableHashSet.Create(
                                        "Default",
                                        "OneTime",
                                        "OneWay",
                                        "OneWayToSource",
                                        "TwoWay"
                                      );

    /// <summary>
    /// Validate attribute name with or without qualification
    /// </summary>
    public static bool IsValidAttribute( NameSyntax? name )
    {
        switch ( name )
        {
            case SimpleNameSyntax simple when simple.Identifier.Text is "BindableProperty" or "BindablePropertyAttribute":
                return true;

            default:
                return name is QualifiedNameSyntax qualified &&
                     qualified.Right.Identifier.Text is "BindableProperty" or "BindablePropertyAttribute" &&
                     qualified.Left is SimpleNameSyntax nameSpace &&
                     nameSpace.Identifier.Text is "BindablePropertyAttributes";
        }
    }

    /// <summary>
    /// Validate the attribute's argument list
    /// </summary>
    public static bool HasInvalidAttributeArguments( AttributeSyntax attributeSyntax, out string? errorArgument, 
                                                                                      out Location? errorLocation )
    {
        errorLocation = null;
        errorArgument = null;

        //  No arguments is a valid result
        if ( attributeSyntax.ArgumentList is null || attributeSyntax.ArgumentList.Arguments.Count == 0 )
            return false;

        foreach ( var argument in attributeSyntax.ArgumentList.Arguments )
        {
            var argumentName =  argument?.NameEquals?.Name.Identifier.Text;

            if ( argumentName is not null && ! ValidAttributeArguments.Contains( argumentName ) )
            {
                errorArgument = argumentName;
                errorLocation = argument?.NameEquals?.Name.GetLocation();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// If this is a BindingMode argument, so check for a valid BindingMode
    /// </summary>
    public static bool IsInvalidBindingMode( AttributeSyntax attributeSyntax, out string? argumentValue, out Location? location )
    {
        location        = null;
        argumentValue   = null;

        if ( attributeSyntax.ArgumentList is not null )
            foreach ( var argument in attributeSyntax.ArgumentList.Arguments )
            {
                var argumentName =  argument?.NameEquals?.Name.Identifier.Text;
                var expr         =  argument?.Expression.GetFirstToken().Text.Unquote();

                if ( argumentName is null || expr is null )
                    continue;

                if ( argumentName == "DefaultBindingMode" )
                {
                    if ( expr.StartsWith( "BindingMode." ) )
                        expr = expr.Substring( expr.IndexOf( '.' ) + 1 );

                    if ( ValidBindingModes.Contains( expr ) )
                        break;

                    argumentValue   = expr;
                    location        = argument?.Expression.GetLocation();
                    return true;
                }
            }

        return false;
    }

    /// <summary>
    /// Used by to determine whether the specified field is valid.
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

    /// <summary>
    /// Given a field, locate its class declaration
    /// </summary>
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
        isFileScoped = false;

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
                namespaceName = namespaceDeclaration.Name.ToString() + "." + namespaceName;
            }
        }

        return namespaceName;
    }
}
