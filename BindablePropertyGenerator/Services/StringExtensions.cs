namespace BindablePropertyGenerator;

using System.Resources;

public static class StringExtensions
{
    public static string ToPascalCase( this string fieldName )
    {
        if ( fieldName.StartsWith( "m_" ) )
            fieldName = fieldName.Substring( 2 );
        else if ( fieldName.StartsWith( "_" ) )
            fieldName = fieldName.Substring( 1 );

        return fieldName.Length == 0 ? "" : fieldName.Substring( 0, 1 ).ToUpper() + fieldName.Substring( 1 );
    }

    /// <summary>
    /// Given a quoted string, strips the outermost quotes while preserving interior quotes
    /// </summary>
    public static string Unquote( this string source )
    {
        var length = source.Length;

        if ( source[ 0 ] == '"' && source[ length - 1 ] == '"' )
            source = source.Substring( 1, length - 2 );

        source = source.Replace( "\\\"", "\"" );

        return source;
    }
}
