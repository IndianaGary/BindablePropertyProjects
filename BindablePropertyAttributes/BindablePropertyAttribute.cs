#nullable enable
namespace BindablePropertyAttributes;

[System.AttributeUsage( System.AttributeTargets.Field, AllowMultiple = false, Inherited = false )]
public sealed class BindablePropertyAttribute : System.Attribute
{
    public string? DeclaringType        { get; set; } = null;
    public string? DefaultBindingMode   { get; set; } = null;
    public string? DefaultValue         { get; set; } = null;
    public string? ValidateValue        { get; set; } = null;
    public string? PropertyChanged      { get; set; } = null;
    public string? PropertyChanging     { get; set; } = null;
    public string? CoerceValue          { get; set; } = null;
    public string? DefaultValueCreator  { get; set; } = null;
    public string? HidesBaseProperty    { get; set; } = "false";
}
#nullable restore
