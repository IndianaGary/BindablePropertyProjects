namespace BindablePropertyGenerator.UnitTests;

using BindablePropertyFeatures;
using BindablePropertyAttributes;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Initialize();
}

public static class TestHelper
{
    public static Task Verify( string source )
    {
        // Parse the provided string into a C# syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText( source );

        //  Create assembly references
        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(BindablePropertyAttribute).Assembly.Location),
        };

        // Create a Roslyn compilation for the syntax tree.
        var compilation = CSharpCompilation.Create( 
                                                    assemblyName: "UnitTest", 
                                                    syntaxTrees: new[] { syntaxTree },
                                                    references: references );

        // Create an instance of our BindableProperty incremental source generator
        var generator = new BindablePropertyGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        var driver = CSharpGeneratorDriver.Create( generator );

        // Run the source generator!
        driver = (CSharpGeneratorDriver)driver.RunGenerators( compilation );

        // Use verify to snapshot test the source generator output!
        return Verifier
              .Verify( driver )
              .UseDirectory( "Snapshots");
    }
}
