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
            namespace UnitTest;

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
    public async Task Should_Fail_Because_BindingMode_Is_Invalid_V1()
    {
        //  Arrange
        var codeToTest = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty( DefaultBindingMode="BadMode") ]
                public Color _backgroundColor;
            }
            """;

        //  Act
        var expected = VerifyCS.Diagnostic( "GLLBP003" ).WithSpan( 9, 24, 9, 52 );

        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest, expected );
    }

    [TestMethod]
    public async Task Should_Fail_Because_BindingMode_Is_Invalid_V2()
    {
        //  Arrange
        var codeToTest = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty( DefaultBindingMode="BindingMode.BadMode") ]
                public Color _backgroundColor;
            }
            """;

        //  Act
        var expected = VerifyCS.Diagnostic( "GLLBP003" ).WithSpan( 9, 24, 9, 64 );

        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest, expected );
    }

    [TestMethod]
    public async Task Should_Succeed_Because_BindingMode_Is_Valid()
    {
        //  Arrange
        var codeToTest = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty( DefaultBindingMode="TwoWay") ]
                public Color _backgroundColor;
            }
            """;

        //  Act
        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest );
    }

    [TestMethod]
    public async Task Should_Succeed_Because_There_Are_No_Errors()
    {
        //  Arrange
        var codeToTest = """
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

        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest );
    }
}
