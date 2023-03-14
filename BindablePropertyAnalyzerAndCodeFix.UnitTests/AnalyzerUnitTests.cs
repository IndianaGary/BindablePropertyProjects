namespace BindablePropertyAnalyzerAndCodeFix.UnitTests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = CSharpCodeFixVerifier<BindablePropertyAnalyzer, BindablePropertyCodeFixProvider>;
[TestClass]
public class AnalyzerUnitTests
{
    [TestMethod]
    public async Task Should_Succeed_Because_There_is_No_Code()
    {
        //  Arrange
        var noCodeToTest = "";

        //  Act
        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( noCodeToTest );
    }

    [TestMethod]
    public async Task Should_Fail_Because_Not_Partial_Class()
    { 
        //  Arrange
        var codeToTest = """
            namespace UnitTest1;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public class TestFile : ContentView
            {
                [BindableProperty]
                public Color _backgroundColor;
            }
            """;

        //  Act
        var expected = VerifyCS.Diagnostic( "GLLBP001" ).WithLocation( 7, 14 ).WithArguments("TestFile");

        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest, expected );
    }

    [TestMethod]
    public async Task Should_Fail_Because_Multiple_Fields_Declared()
    {
        //  Arrange
        var codeToTest = """
            namespace UnitTest2;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty]
                public Color _backgroundColor, _color2;
            }
            """;

        //  Act
        var expected = VerifyCS.Diagnostic( "GLLBP002" ).WithSpan( 10, 18, 10, 44 );

        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest, expected );
    }

    [TestMethod]
    public async Task Should_Succeed_Because_There_Are_No_Errors()
    {
        //  Arrange
        var codeToTest = """
            namespace UnitTest3;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty]
                public Color _backgroundColor;
            }
            """;

        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest );
    }
}
