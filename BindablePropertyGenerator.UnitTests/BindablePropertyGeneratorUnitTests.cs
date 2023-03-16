namespace BindablePropertyGenerator.UnitTests;

[UsesVerify]
public class BindablePropertyGeneratorUnitTests
{
    [Fact]
    public async Task Generates_Nothing_Because_No_Partial_Class()
    {
        var source = """
        namespace UnitTest;

        using BindablePropertyAttributes;
        
        public class Test_Partial_Class
        {
            [BindableProperty]
            public Color   _backgroundColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_Nothing_Because_Too_Many_Fields()
    {
        var source = """
        namespace UnitTest;

        using BindablePropertyAttributes;
        
        public class Test_Too_Many_Fields
        {
            [BindableProperty]
            public Color   _backgroundColor, _anotherColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_File_Scoped_Namespace()
    {
        var source = """
        namespace UnitTest;

        using BindablePropertyAttributes;
        
        public partial class Test_FileScoped_Namespace
        {
            [BindableProperty]
            public Color   _backgroundColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_Inline_Namespace()
    {
        var source = """
        using BindablePropertyAttributes;
    
        namespace UnitTest
        {
            public partial class Test_Inline_Namespace
            {
                [BindableProperty]
                public Color   _foregroundColor;
            }
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_Inline_Nested_Namespace()
    {
        var source = """
        using BindablePropertyAttributes;
    
        namespace UnitTest
        {
            namespace UnitTestInner
            {
                public partial class Test_Inline_Nested_Namespace
                {
                    [BindableProperty]
                    public Color   _textColor;
                }
            }
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_Full_Attribute_Specification()
    {
        var source = """
        using BindablePropertyAttributes;
    
        namespace UnitTest
        {
            public partial class Test_Full_Attribute
            {
                [BindablePropertyAttribute]
                public Color   _textColor;
            }
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_Fully_Qualified_Attribute_Specification()
    {
        var source = """
        namespace UnitTest
        {
            public partial class Test_Fully_Qualified_Attribute
            {
                [BindablePropertyAttributes.BindablePropertyAttribute]
                public Color   _textColor;
            }
        }
        """;

        await TestHelper.Verify( source );
    }
    [Fact]
    public async Task Generates_DefaultValue_Argument()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_DefaultValue
        {
            [BindableProperty( DefaultValue = "Colors.Green" )]
            public Color   _backDropColor1;
    
            [BindableProperty( DefaultValue= "Color.FromArgb(\"#4B000000\")" )]
            public Color   m_backDropColor2;
        
            [BindableProperty( DefaultValue= @"Color.FromArgb(""#5C000000"")" )]
            public Color   backDropColor3;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_DeclaringType_Override()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_DeclaringType
        {
            [BindableProperty( DeclaringType = "ContentView" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_BindingMode_Default()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_DefaultBindingMode_Abbreviated
        {
            [BindableProperty( DefaultBindingMode = "TwoWay" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_BindingMode_Full()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_DefaultBindingMode_Full
        {
            [BindableProperty( DefaultBindingMode = "BindingMode.TwoWay" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_ValidateValue()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_ValidateValue
        {
            [BindableProperty( ValidateValue = "OnValidateBorderColor" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_PropertyChanged()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_PropertyChanged
        {
            [BindableProperty( PropertyChanged = "OnBorderColorChanged" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_PropertyChanging()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_PropertyChanging
        {
            [BindableProperty( PropertyChanging = "OnBorderColorChanging" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_CoerceValue()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_CoerceValue
        {
            [BindableProperty( CoerceValue = "OnCoerceBorderColorValue" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_DefaultValueCreator()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_DefaultValueCreator
        {
            [BindableProperty( DefaultValueCreator = "OnCreateBorderColorDefaultValue" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_HidesBaseProperty()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_HidesBaseProperty
        {
            [BindableProperty( HidesBaseProperty = "true" )]
            public Color   _borderColor;

            [BindableProperty( HidesBaseProperty = "false" )]
            public Color   _extraColor;
        }
        """;

        await TestHelper.Verify( source );
    }

    [Fact]
    public async Task Generates_All_BaseProperty()
    {
        var source = """
        namespace UnitTest;
    
        using BindablePropertyAttributes;
    
        public partial class Test_All_Properties
        {
            [BindableProperty( HidesBaseProperty="true", DeclaringType="ContentView", DefaultBindingMode="OneTime",
                               DefaultValue="Colors.Red", ValidateValue="OnValidateBorderColor",
                               PropertyChanged="OnBorderColorChanged", PropertyChanging="OnBorderColorChanging",
                               CoerceValue="OnCoerceBorderColorValue", DefaultValueCreator="OnCreateDefaultBorderColorValue" )]
            public Color   _borderColor;
        }
        """;

        await TestHelper.Verify( source );
    }
}
