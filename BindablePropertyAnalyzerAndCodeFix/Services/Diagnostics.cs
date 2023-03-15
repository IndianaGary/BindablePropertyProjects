namespace BindablePropertyAnalyzerAndCodeFix;

using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

#pragma warning disable IDE1006 // Naming Styles
public static class Diagnostics
{
    public static readonly DiagnosticDescriptor ClassMustBePartial
           = new( "GLLBP001",                                 // id
                  "Class must be partial",                    // title
                  "Class {0} contains generated code and must be declared as 'partial'",    // message
                  "BindablePropertyAnalyzer",                 // category
                  DiagnosticSeverity.Error,
                  true );

    public static readonly DiagnosticDescriptor AmbiguousFieldNames
           = new( "GLLBP002",                                 // id
                  "Ambiguous field names specified",          // title
                  "Field names after {0} are ambiguous",      // message
                  "BindablePropertyAnalyzer",                 // category
                  DiagnosticSeverity.Error,
                  true );

    public static readonly DiagnosticDescriptor InvalidBindingMode
           = new( "GLLBP003",                                // id
                  "Invalid BindingMode",                     // title
                  "Argument must be a valid BindingMode",    // message
                  "BindablePropertyAnalyzer",                // category
                  DiagnosticSeverity.Error,
                  true );

    public static readonly DiagnosticDescriptor InvalidArgument
           = new( "GLLBP004",                                // id
                  "Invalid argument",                        // title
                  "{0} is not a valid argument",             // message
                  "BindablePropertyAnalyzer",                // category
                  DiagnosticSeverity.Error,
                  true );

    public static readonly ImmutableArray<DiagnosticDescriptor> Descriptors
        =   ImmutableArray.Create(
                                   Diagnostics.ClassMustBePartial,
                                   Diagnostics.AmbiguousFieldNames,
                                   Diagnostics.InvalidBindingMode,
                                   Diagnostics.InvalidArgument
                                 );
}
#pragma warning restore IDE1006 // Naming Styles
