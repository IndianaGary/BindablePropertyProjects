﻿namespace BindablePropertyAnalyzerAndCodeFix.UnitTests;

using BindablePropertyAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

public static partial class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic()"/>
    public static DiagnosticResult Diagnostic()
        => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic();

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(string)"/>
    public static DiagnosticResult Diagnostic( string diagnosticId )
        => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic( diagnosticId );

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
    public static DiagnosticResult Diagnostic( DiagnosticDescriptor descriptor )
        => CSharpAnalyzerVerifier<TAnalyzer, MSTestVerifier>.Diagnostic( descriptor );

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
    public static async Task VerifyAnalyzerAsync( string source, params DiagnosticResult[] expected )
    {
        var test = new Test
        {
            TestCode = source,
            ReferenceAssemblies = new ReferenceAssemblies( "net7.0",
                                                             new PackageIdentity( "Microsoft.NETCore.App.Ref", "7.0.3" ),
                                                             Path.Combine( "ref", "net7.0" ) )
        };

        test.TestState.AdditionalReferences.Add( typeof( Button ).Assembly );
        test.TestState.AdditionalReferences.Add( typeof( Color ).Assembly );
        test.TestState.AdditionalReferences.Add( typeof( BindablePropertyAttribute ).Assembly );
        test.TestState.ReferenceAssemblies = null;
        test.ExpectedDiagnostics.AddRange( expected );
        await test.RunAsync( CancellationToken.None );
    }
}
