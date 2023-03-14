namespace BindablePropertyAnalyzerAndCodeFix;

using BindablePropertyAnalyzerAndCodeFix.CodeFix;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[ExportCodeFixProvider( LanguageNames.CSharp, Name = nameof( BindablePropertyCodeFixProvider ) ), Shared]
public class BindablePropertyCodeFixProvider : CodeFixProvider
{
    /// <summary>
    /// Localizable descriptor ids
    /// </summary
    public static ImmutableArray<string> LocalizableDescriptorTitles 
            => ImmutableArray.Create(
                                      CodeFixResources.GLLBP001,
                                      CodeFixResources.GLLBP002
                                    );

    private static ImmutableArray<Func<Document, CSharpSyntaxNode, CancellationToken, Task<Document>>> CodeFixes 
            => ImmutableArray.Create(
                                      MakePartialAsync,
                                      RemoveAmbiguousFields
                                    );

    /// <summary>
    /// List of available correctable Diagnostics 
    /// </summary>
    public sealed override ImmutableArray<string> FixableDiagnosticIds 
            => ImmutableArray.Create(
                                      Diagnostics.ClassMustBePartial.Id,
                                      Diagnostics.AmbiguousFieldNames.Id
                                    );

    /// <summary>
    /// See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md 
    /// for more information on Fix All Providers
    /// </summary>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <summary>
    /// Register all codefixes we handle
    /// </summary>
    public sealed override async Task RegisterCodeFixesAsync( CodeFixContext context )
    {
        foreach ( var diagnostic in context.Diagnostics )
        {
            for ( var i = 0; i < FixableDiagnosticIds.Length; i++ )
                if ( diagnostic.Id == FixableDiagnosticIds[ i ] )
                    await RegisterNewCodeFixAsync( context, diagnostic, i ).ConfigureAwait( false );
        }
    }

    async Task RegisterNewCodeFixAsync( CodeFixContext context, Diagnostic diagnostic, int descriptorIndex)
    {
        var root                =   await context.Document.GetSyntaxRootAsync( context.CancellationToken );
        var diagnosticSpan      =   diagnostic.Location.SourceSpan;
        var title               =   LocalizableDescriptorTitles[ descriptorIndex ];

        var classDeclaration    =   root?.FindToken( diagnosticSpan.Start).Parent?.AncestorsAndSelf()
                                                                          .OfType<ClassDeclarationSyntax>().First();
        if ( classDeclaration is not null && diagnostic.Id == "GLLBP001" )
        { 
            var codeAction  =  CodeAction.Create( title, 
                                                  async ct => await CodeFixes[ descriptorIndex ]( context.Document, classDeclaration, ct ).ConfigureAwait( false ),
                                                  diagnostic.Id );
            context.RegisterCodeFix( codeAction, diagnostic );
            return;
        }

        var fieldDeclaration    =   root?.FindToken( diagnosticSpan.Start).Parent?.AncestorsAndSelf()
                                                                          .OfType<FieldDeclarationSyntax>().First();
        if ( fieldDeclaration is not null && diagnostic.Id == "GLLBP002" )
        {
            var codeAction  =  CodeAction.Create( title,
                                                  async ct => await CodeFixes[ descriptorIndex ]( context.Document, fieldDeclaration, ct ).ConfigureAwait( false ),
                                                  diagnostic.Id );
            context.RegisterCodeFix( codeAction, diagnostic );
            return;
        }
    }

    static async Task<Document> MakePartialAsync( Document document, CSharpSyntaxNode typeDeclaration, CancellationToken cancellationToken )
    {
        var classDeclaration    =   (ClassDeclarationSyntax)typeDeclaration;
        var updatedModifiers    =   classDeclaration.Modifiers.Add( SyntaxFactory.Token(SyntaxKind.PartialKeyword) );
        var newClassDeclaration =   classDeclaration.WithModifiers( updatedModifiers );

        var oldRoot             =   await document.GetSyntaxRootAsync( cancellationToken ).ConfigureAwait( false );
        var newRoot             =   oldRoot!.ReplaceNode( classDeclaration, newClassDeclaration );
        return document.WithSyntaxRoot( newRoot );
    }

    static async Task<Document> RemoveStaticAsync( Document document, CSharpSyntaxNode typeDeclaration, CancellationToken cancellationToken )
    {
        var classDeclaration    =   (ClassDeclarationSyntax)typeDeclaration;
        var updatedModifiers    =   classDeclaration.Modifiers.Remove( SyntaxFactory.Token(SyntaxKind.StaticKeyword) );
        var newClassDeclaration =   classDeclaration.WithModifiers( updatedModifiers );

        var oldRoot             =   await document.GetSyntaxRootAsync( cancellationToken ).ConfigureAwait( false );
        var newRoot             =   oldRoot!.ReplaceNode( classDeclaration, newClassDeclaration );
        return document.WithSyntaxRoot( newRoot );
    }

    static async Task<Document> RemoveAmbiguousFields( Document document, CSharpSyntaxNode typeDeclaration, CancellationToken cancellationToken )
    {
        var fieldDeclaration    =   (FieldDeclarationSyntax)typeDeclaration;
        var firstVariable       =   fieldDeclaration.Declaration.Variables[ 0 ];
        var newVariableList     =   new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add( firstVariable );
        var updatedVariables    =   fieldDeclaration.Declaration.WithVariables( newVariableList );
        var newFieldDeclaration =   fieldDeclaration.WithDeclaration( updatedVariables );

        var oldRoot             =   await document.GetSyntaxRootAsync( cancellationToken ).ConfigureAwait( false );
        var newRoot             =   oldRoot!.ReplaceNode( fieldDeclaration, newFieldDeclaration );
        return document.WithSyntaxRoot( newRoot );
    }
}
