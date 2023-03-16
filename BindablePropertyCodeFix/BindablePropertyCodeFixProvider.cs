namespace BindablePropertyCodeFix;

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
            => ImmutableArray.Create( CodeFixResources.GLLBP001, CodeFixResources.GLLBP002 );

    /// <summary>
    /// List of available correctable Diagnostics 
    /// </summary>
    public sealed override ImmutableArray<string> FixableDiagnosticIds 
            => ImmutableArray.Create( "GLLBP001", "GLLBP002" );

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

        switch ( diagnostic.Id )
        {
            case "GLLBP001":
            {
                var classDeclaration    =   root?.FindToken( diagnosticSpan.Start )
                                                 .Parent?.AncestorsAndSelf()
                                                 .OfType<ClassDeclarationSyntax>().First();

                if ( classDeclaration is null )
                    return;

                var codeAction  =  CodeAction.Create( title,
                                                      async ct => await MakePartialAsync( context.Document, classDeclaration, ct ),
                                                      diagnostic.Id );

                context.RegisterCodeFix( codeAction, diagnostic );
                break;
            }

            case "GLLBP002":
            {
                var fieldDeclaration    =   root?.FindToken( diagnosticSpan.Start )
                                                 .Parent?.AncestorsAndSelf()
                                                 .OfType<FieldDeclarationSyntax>().First();

                if ( fieldDeclaration is null )
                    return;

                var codeAction  =  CodeAction.Create( title,
                                                      async ct => await RemoveAmbiguousFields( context.Document, fieldDeclaration, ct ),
                                                      diagnostic.Id );

                context.RegisterCodeFix( codeAction, diagnostic );
                break;
            }
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
