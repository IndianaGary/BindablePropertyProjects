namespace BindablePropertyFeatures;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using BindablePropertyServices;

[Generator( LanguageNames.CSharp )]
public sealed class BindablePropertyGenerator : IIncrementalGenerator
{
    const string    _mauiControlsAssembly   = "Microsoft.Maui.Controls";
    const string    _mauiGraphicsAssembly   = "Microsoft.Maui.Graphics";
    public string?  _namespaceName          = null;
    public bool     _isFileScopedNamespace  = false; 
    public string?  _version;

    public BindablePropertyGenerator() => _version = typeof( BindablePropertyGenerator )
                                                     .Assembly
                                                     .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                     .InformationalVersion;

    /// <summary>
    ///  Source generator
    /// </summary>
    public void Initialize( IncrementalGeneratorInitializationContext initContext )
    {
        //  Look for fields that have a BindbaleProperty attribute applied
        var bpModels = initContext.SyntaxProvider.CreateSyntaxProvider(
                                                        predicate: (node, _ )     =>  IsAttributeOrClassDeclaration( node ),
                                                        transform: (context, _ )  =>  BuildGenerationModel( context ) )
                                                 .Where( static model => model is not null && model.IsInitialized )
                                                 .Collect(); 

        //  Generate code uaing the compilation and fields
        initContext.RegisterSourceOutput( bpModels, GenerateCode );
    }

    /// <summary>
    /// Looks for an attribute declaration or a class declaration
    /// </summary>
    bool IsAttributeOrClassDeclaration( SyntaxNode node )
    {
        switch ( node )
        {
            case AttributeSyntax attribute:
                return Helpers.IsValidAttribute( attribute.Name ) &&
                       ! Helpers.HasInvalidAttributeArguments( attribute, out var _, out var _  ) &&
                       ! Helpers.IsInvalidBindingMode( attribute, out var _, out var _ );

            case ClassDeclarationSyntax cls:
                if ( cls.IsValidClassDeclaration() && _namespaceName is null )
                {
                    _namespaceName = Helpers.GetFullyQualifiedNamespaceName( cls, out var isFileScoped );
                    _isFileScopedNamespace = isFileScoped;
                }
                break;

            default:
                break;
        }

        return false;
    }

    /// <summary>
    /// Builds the model used to generate code when valid
    /// </summary>
    BindablePropertyModel BuildGenerationModel( GeneratorSyntaxContext context )
    {
        //  Creates a default (uninitialized) model used to collect data for code generation
        var model           =   new BindablePropertyModel();

        //  If we get here, a namespace must have been established
        if ( _namespaceName is null )
            return model;

        //  Only interested in attributes
        if ( context.Node is not AttributeSyntax attributeSyntax )
            return model;

        //  ... and only attributes applied to fields
        if ( attributeSyntax.Parent?.Parent is not FieldDeclarationSyntax fieldDeclaration )
            return model;

        //  ... and only field declarations with one field name
        if ( fieldDeclaration.Declaration.Variables.Count != 1 )
            return model;

        //  Find the symbol in the semantic model for the field
        if ( context.SemanticModel.GetDeclaredSymbol( fieldDeclaration.Declaration.Variables[ 0 ] ) is not IFieldSymbol fieldType )
            return model;

        //  Only handle MY BindableProperty attributes
        if ( ! Helpers.IsValidFieldSymbol( fieldType ) )
            return model;

        //  Fill in information from the field
        model.IsInitialized =   true;
        model.Namespace     =   _namespaceName;
        model.ClassName     =   fieldType.ContainingType.ToString();
        model.Accessibility =   fieldType.DeclaredAccessibility.ToString().ToLower();
        model.TypeName      =   fieldType.Type.ToString();
        model.POCName       =   fieldType.Name.ToString().ToPascalCase();

        if ( model.ClassName.Contains( '.' ) )
            model.ClassName = model.ClassName.Substring( model.ClassName.LastIndexOf( '.' ) + 1 );

        //  Fill in information from the attribute
        if ( attributeSyntax.ArgumentList is not null )
            foreach ( var argument in attributeSyntax.ArgumentList.Arguments )
                CollectAttributeArguments( argument, model );

        //  Default to class name if not overridden
        model.DeclaringType ??= model.ClassName;

        return model;
    }

    /// <summary>
    /// Prepare to generate code
    /// </summary>
    void GenerateCode( SourceProductionContext context, ImmutableArray<BindablePropertyModel> models )
    { 
        if ( models.IsDefaultOrEmpty )
            return;

        var indent = _isFileScopedNamespace ? "" : "    ";

        foreach ( var model in models )
        { 
            if ( ! model.IsInitialized )
                continue;

            var sb      =   new StringBuilder();
            var count   =   model.NumberOfOptionalArgumentsAssigned();
            var hides   =   model.HidesBaseProperty.ToLower() == "true" ? "new " : "";

            sb.AppendLine( $"//  <auto-generated - BindablePropertyGenerator Version: {_version} />" );

            if ( _isFileScopedNamespace )
                sb.AppendLine( $"namespace {model.Namespace};" );

            sb.AppendLine();
            sb.AppendLine( $"using {_mauiControlsAssembly};" );
            sb.AppendLine( $"using {_mauiGraphicsAssembly};" );
            sb.AppendLine();

            if ( ! _isFileScopedNamespace )
            {
                sb.AppendLine( $"namespace {model.Namespace}" );
                sb.AppendLine( "{" );
            }

            sb.AppendLine( $"{indent}public partial class {model.ClassName}" );
            sb.AppendLine( $"{indent}{{");
            sb.AppendLine( $"{indent}    {model.Accessibility} static {hides}readonly BindableProperty {model.POCName}Property" );
            sb.AppendLine( $"{indent}           = BindableProperty.Create( nameof({model.POCName})," );
            sb.AppendLine( $"{indent}                                      typeof({model.TypeName})," );
                sb.Append( $"{indent}                                      typeof({model.DeclaringType})" );

            if ( count > 0 )
            {
                if ( model.DefaultBindingMode is not null )
                {
                    var bindingMode = model.DefaultBindingMode.StartsWith( "BindingMode." ) ? model.DefaultBindingMode 
                                                                                            : "BindingMode." + model.DefaultBindingMode;

                    sb.AppendLine( "," );
                    sb.Append( $"{indent}                                      defaultBindingMode: {bindingMode}" );
                }

                if ( model.DefaultValue is not null )
                {
                    sb.AppendLine( "," );
                    sb.Append( $"{indent}                                      defaultValue: {model.DefaultValue}" );
                }

                if ( model.ValidateValue is not null )
                {
                    sb.AppendLine( "," );
                    sb.Append( $"{indent}                                      validateValue: {model.ValidateValue}" );
                }

                if ( model.PropertyChanged is not null )
                {
                    sb.AppendLine( "," );
                    sb.Append( $"{indent}                                      propertyChanged: {model.PropertyChanged}" );
                }

                if ( model.PropertyChanging is not null )
                {
                    sb.AppendLine( "," );
                    sb.Append( $"{indent}                                      propertyChanging: {model.PropertyChanging}" );
                }

                if ( model.CoerceValue is not null )
                {
                    sb.AppendLine( "," );
                    sb.Append( $"{indent}                                      coerceValue: {model.CoerceValue}" );
                }

                if ( model.DefaultValueCreator is not null )
                {
                    sb.AppendLine( "," );
                    sb.Append( $"{indent}                                      defaultValueCreator: {model.DefaultValueCreator}" );
                }
            }

            sb.AppendLine();
            sb.AppendLine( $"{indent}                                    );" );

            sb.AppendLine( $"{indent}    {model.Accessibility} {hides}{model.TypeName} {model.POCName}" );
            sb.AppendLine( $"{indent}    {{" );
            sb.AppendLine( $"{indent}        get => ({model.TypeName})GetValue( {model.POCName}Property );" );
            sb.AppendLine( $"{indent}        set => SetValue( {model.POCName}Property, value );" );
            sb.AppendLine( $"{indent}    }}" );
            sb.AppendLine( $"{indent}}}" );

            if ( ! _isFileScopedNamespace )
                sb.AppendLine( "}" );

            var fileName    =   model.ClassName + "_" + model.POCName + ".g.cs";
            var text        =   sb.ToString();
            context.AddSource( fileName, text );
        }
    }

    /// <summary>
    /// Fill in the model from user specified arguments
    /// </summary>
    void CollectAttributeArguments( AttributeArgumentSyntax? node, BindablePropertyModel model )
    {
        if ( node is null || node.NameEquals is null )
            return;

        var argumentName =  node.NameEquals.Name.Identifier.Text;
        var expr         =  node.Expression.GetFirstToken().Text.Unquote();

        switch ( argumentName )
        {
            case "DeclaringType":       model.DeclaringType       = expr;           break;
            case "DefaultBindingMode":  model.DefaultBindingMode  = expr;           break;
            case "DefaultValue":        model.DefaultValue        = expr;           break;
            case "ValidateValue":       model.ValidateValue       = expr;           break;
            case "PropertyChanged":     model.PropertyChanged     = expr;           break;
            case "PropertyChanging":    model.PropertyChanging    = expr;           break;
            case "CoerceValue":         model.CoerceValue         = expr;           break;
            case "DefaultValueCreator": model.DefaultValueCreator = expr;           break;
            case "HidesBaseProperty":   model.HidesBaseProperty   = expr.ToLower(); break;

            default: break;
        }
    }
}
