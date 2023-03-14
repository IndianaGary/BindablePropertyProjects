namespace BindablePropertyAnalyzerAndCodeFix.UnitTests;

using BindablePropertyAttributes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

public static partial class CSharpAnalyzerVerifier<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    public class Test : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
    {
        public Test()
        {
            TestState.AdditionalReferences.Add( typeof( BindablePropertyAttribute ).Assembly );

            SolutionTransforms.Add( ( solution, projectId ) =>
            {
                var project     = solution.GetProject(projectId);
                var compOptions = project.CompilationOptions;
                var diagOptions = compOptions.SpecificDiagnosticOptions.SetItems( CSharpVerifierHelper.NullableWarnings );
                compOptions     = compOptions.WithSpecificDiagnosticOptions( diagOptions );
                solution        = solution.WithProjectCompilationOptions( projectId, compOptions );

                return solution;
            } );
        }
    }
}
