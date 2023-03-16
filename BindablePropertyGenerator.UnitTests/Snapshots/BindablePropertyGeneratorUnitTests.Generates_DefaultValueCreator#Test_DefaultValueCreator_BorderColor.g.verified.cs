﻿//HintName: Test_DefaultValueCreator_BorderColor.g.cs
//  <auto-generated - BindablePropertyGenerator Version: 0.6.7.0-beta />
namespace UnitTest;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

public partial class Test_DefaultValueCreator
{
    public static readonly BindableProperty BorderColorProperty
           = BindableProperty.Create( nameof(BorderColor),
                                      typeof(Color),
                                      typeof(Test_DefaultValueCreator),
                                      defaultValueCreator: OnCreateBorderColorDefaultValue
                                    );
    public Color BorderColor
    {
        get => (Color)GetValue( BorderColorProperty );
        set => SetValue( BorderColorProperty, value );
    }
}
