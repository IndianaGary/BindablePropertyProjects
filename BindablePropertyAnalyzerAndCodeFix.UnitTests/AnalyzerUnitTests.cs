namespace BindablePropertyAnalyzerAndCodeFix.UnitTests;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = CSharpCodeFixVerifier<BindablePropertyFeatures.BindablePropertyAnalyzer,
                                       BindablePropertyCodeFix.BindablePropertyCodeFixProvider>;

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
    public async Task Should_Fail_Because_Attribute_Argument_Is_Invalid()
    {
        //  Arrange
        var codeToTest = """
            namespace UnitTest;

            using BindablePropertyAttributes;
            using Microsoft.Maui.Controls;
            using Microsoft.Maui.Graphics;
            
            public partial class TestFile : ContentView
            {
                [BindableProperty( InvalidArgument="true" ) ]
                public Color _backgroundColor, _color2;
            }
            """;
        DiagnosticDescriptor expected1 = new
            (
              id: "GLLBP004",
              title: "",
              messageFormat: "{0} is not a valid BindableProperty attribute argument",
              defaultSeverity: DiagnosticSeverity.Error,
              category: "BindablePropertyAnalyzer",
              isEnabledByDefault: true 
            );

        DiagnosticDescriptor expected2 = new
            (
              id: "CS0246",
              title: "",
              messageFormat: "The type or namespace name '{0}' could not be found (are you missing a using directive or an assembly reference?)",
              defaultSeverity: DiagnosticSeverity.Error,
              category: "BindablePropertyAnalyzer",
              isEnabledByDefault: true
            );


        //  Act
        var myError1     = VerifyCS.Diagnostic( expected1 ).WithSpan( 9, 24, 9, 39 ).WithArguments("InvalidArgument");
        var myError2     = VerifyCS.Diagnostic( expected2 ).WithSpan( 9, 24, 9, 39 ).WithArguments("InvalidArgument");

        //  Assert
        await VerifyCS.VerifyAnalyzerAsync( codeToTest, myError1, myError2, myError2 );
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
        var expected = VerifyCS.Diagnostic( "GLLBP003" ).WithSpan( 9, 43, 9, 52 ).WithArguments("BadMode");

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
        var expected = VerifyCS.Diagnostic( "GLLBP003" ).WithSpan( 9, 43, 9, 64 ).WithArguments("BadMode"); ;

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
