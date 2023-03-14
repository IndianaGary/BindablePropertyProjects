namespace BindablePropertyAnalyzerAndCodeFix;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Reflection;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public class BindablePropertyAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// List of available Diagnostics 
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = Diagnostics.Descriptors;

    public HashSet<string>  ClassNameLookup { get; set;}

    /// <summary>
    /// Constructor
    /// </summary>
    public BindablePropertyAnalyzer() => ClassNameLookup = new HashSet<string>();

    /// <summary>
    /// Analyzer initialization
    /// </summary>
    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.None );
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction( AnalyzeSyntax, SyntaxKind.Attribute );
    }

    void AnalyzeSyntax( SyntaxNodeAnalysisContext context )
    {
        if ( context.Node is AttributeSyntax attributeSyntax )
        {
            //  See if this is a [BindableProperty] attribute
            if ( ! SyntaxHelpers.IsValidAttribute( attributeSyntax.Name) )
                return;

            //  Now, see if it is parented by a field
            if ( attributeSyntax.Parent?.Parent is not FieldDeclarationSyntax fieldDeclaration )
                return;

            //  Extract the symbol for the first field declared from the semantic model
            if ( context.SemanticModel.GetDeclaredSymbol( fieldDeclaration.Declaration.Variables[ 0 ] ) is not IFieldSymbol firstField )
                return;

            //  See if decorated with the MY BindableProperty attribute
            if ( ! SyntaxHelpers.IsValidFieldSymbol( firstField ) )
                return;

            //  Get the number of fields declared in the statement
            var variableCount   = fieldDeclaration.Declaration.Variables.Count;

            //  More than one field declaration is ambiguous
            if ( variableCount != 1 )
            {
                var start       = fieldDeclaration.Declaration.Variables[ 0 ].FullSpan.Start;
                var end         = fieldDeclaration.Declaration.Variables[ variableCount - 1 ].FullSpan.End;
                var newFullSpan = new TextSpan( start, end - start + 1 );
                var location    = Location.Create( fieldDeclaration.SyntaxTree, newFullSpan );

                var error       = Diagnostic.Create( Diagnostics.AmbiguousFieldNames, location );

                context.ReportDiagnostic( error );
                return;
            }

            //  Get the class the field is declared in
            if ( SyntaxHelpers.TryGetDeclaringClass( fieldDeclaration, out ClassDeclarationSyntax? parentClass ) )
            {
                if ( parentClass is null )
                    return;

                var className               = parentClass.Identifier.ToString();
                var fullyQualifiedClassName = SyntaxHelpers.GetFullyQualifiedNamespaceName( parentClass, out bool _ )
                                            + "." + className;

                //  If class name is cached, it's already been reported, skip it
                if ( ClassNameLookup.Contains( fullyQualifiedClassName ) )
                    return;

                ClassNameLookup.Add( fullyQualifiedClassName );

                //  If not a partial class, generate an error
                if ( ! parentClass!.IsPartialClass() )
                {
                    var error = Diagnostic.Create( Diagnostics.ClassMustBePartial,
                                                   parentClass!.Identifier.GetLocation(),
                                                   className );
                    context.ReportDiagnostic( error );
                }
            }
        }
    }
}
