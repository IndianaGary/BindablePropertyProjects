﻿namespace BindablePropertyGenerator;

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
                  "Mulltiple field names are ambiguous",      // message
                  "BindablePropertyAnalyzer",                 // category
                  DiagnosticSeverity.Error,
                  true );

    public static readonly ImmutableArray<DiagnosticDescriptor> Descriptors
        =   ImmutableArray.Create(
                                   Diagnostics.ClassMustBePartial,
                                   Diagnostics.AmbiguousFieldNames
                                 );
}
#pragma warning restore IDE1006 // Naming Styles