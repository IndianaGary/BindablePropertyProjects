﻿//HintName: Test_ValidateValue_BorderColor.g.cs
//  <auto-generated - BindablePropertyGenerator Version: 0.6.7.0-beta />
namespace UnitTest;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

public partial class Test_ValidateValue
{
    public static readonly BindableProperty BorderColorProperty
           = BindableProperty.Create( nameof(BorderColor),
                                      typeof(Color),
                                      typeof(Test_ValidateValue),
                                      validateValue: OnValidateBorderColor
                                    );
    public Color BorderColor
    {
        get => (Color)GetValue( BorderColorProperty );
        set => SetValue( BorderColorProperty, value );
    }
}
