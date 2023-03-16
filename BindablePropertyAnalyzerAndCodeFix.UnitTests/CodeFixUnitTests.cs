namespace BindablePropertyAnalyzerAndCodeFix.UnitTests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = CSharpCodeFixVerifier<BindablePropertyFeatures.BindablePropertyAnalyzer, 
                                       BindablePropertyCodeFix.BindablePropertyCodeFixProvider>;

[TestClass]
public class CodeFixUnitTests
{
    [TestMethod]
    public async Task Should_Fix_By_Adding_Partial_To_Class()
    {
        //  Arrange
        var codeToTest = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public class TestFile : ContentView
            {
                [BindableProperty]
                public Color _backgroundColor;
            }
            """;

        var codeFix = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty]
                public Color _backgroundColor;
            }
            """;

        //  Act
        var expected = VerifyCS.Diagnostic( "GLLBP001" ).WithLocation( 7, 14 ).WithArguments("TestFile");

        //  Assert
        await VerifyCS.VerifyCodeFixAsync( codeToTest, expected, codeFix );
    }

    [TestMethod]
    public async Task Should_Fix_By_Removing_Extra_Fields()
    {
        var codeToTest = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty]
                public Color _backgroundColor, _color2, color3;
            }
            """;

        var codeFix = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty]
                public Color _backgroundColor;
            }
            """;

        //  Act
        var expected = VerifyCS.Diagnostic( "GLLBP002" ).WithSpan( 10, 18, 10, 52 );

        //  Assert
        await VerifyCS.VerifyCodeFixAsync( codeToTest, expected, codeFix );
    }
}
