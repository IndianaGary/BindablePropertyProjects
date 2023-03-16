namespace BindablePropertyServices;

/// <summary>
/// Data collected for a bindable property to support code generation
/// </summary>
internal class BindablePropertyModel
{
    internal bool   IsInitialized           { get; set; } = false;

    //  From syntax tree
    internal string Namespace               { get; set; } = "";     //  Containing namespace
    internal string ClassName               { get; set; } = "";     //  Containing class name

    //  From field the attribute was applied to
    internal string Accessibility           { get; set; } = "";     //  Accessibility from decorated field
    internal string TypeName                { get; set; } = "";     //  Type from field declaration
    internal string POCName                 { get; set; } = "";     //  Pascal cased version of Plain Old C property name of field name

    //  From attribute (with default values)
    internal string? DeclaringType          { get; set; } = null;    //  typeof(ClassName) or user specified
    internal string? DefaultBindingMode     { get; set; } = null;    //  Optional, User specified or null to denote default value
    internal string? DefaultValue           { get; set; } = null;    //  Optional, User specified or null to denote default value
    internal string? ValidateValue          { get; set; } = null;    //  Optional, User specified or null to denote default value
    internal string? PropertyChanged        { get; set; } = null;    //  Optional, User specified or null to denote default value
    internal string? PropertyChanging       { get; set; } = null;    //  Optional, User specified or null to denote default value
    internal string? CoerceValue            { get; set; } = null;    //  Optional, User specified or null to denote default value
    internal string? DefaultValueCreator    { get; set; } = null;    //  Optional, User specified or null to denote default value
    internal string  HidesBaseProperty      { get; set; } = "false"; //  Optional, User specified or default value of false;

    internal int    NumberOfOptionalArgumentsAssigned()
    {
        var count = 0;

        count +=  DefaultBindingMode    is null ? 0 : 1;
        count +=  DefaultValue          is null ? 0 : 1;
        count +=  ValidateValue         is null ? 0 : 1;
        count +=  PropertyChanged       is null ? 0 : 1;
        count +=  PropertyChanging      is null ? 0 : 1;
        count +=  CoerceValue           is null ? 0 : 1;
        count +=  DefaultValueCreator   is null ? 0 : 1;
        count +=  HidesBaseProperty == "true"   ? 1 : 0;
        
        return count;
    }
}
