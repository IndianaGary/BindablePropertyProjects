﻿//HintName: Test_DefaultValue_BackDropColor3.g.cs
//  <auto-generated - BindablePropertyGenerator Version: 0.6.8.0-beta />
namespace UnitTest;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

public partial class Test_DefaultValue
{
    public static readonly BindableProperty BackDropColor3Property
           = BindableProperty.Create( nameof(BackDropColor3),
                                      typeof(Color),
                                      typeof(Test_DefaultValue),
                                      defaultValue: @"Color.FromArgb(""#5C000000"")"
                                    );
    public Color BackDropColor3
    {
        get => (Color)GetValue( BackDropColor3Property );
        set => SetValue( BackDropColor3Property, value );
    }
}
