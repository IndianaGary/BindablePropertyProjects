﻿//HintName: Test_FileScoped_Namespace_BackgroundColor.g.cs
//  <auto-generated - BindablePropertyGenerator Version: 0.6.8.0-beta />
namespace UnitTest;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

public partial class Test_FileScoped_Namespace
{
    public static readonly BindableProperty BackgroundColorProperty
           = BindableProperty.Create( nameof(BackgroundColor),
                                      typeof(Color),
                                      typeof(Test_FileScoped_Namespace)
                                    );
    public Color BackgroundColor
    {
        get => (Color)GetValue( BackgroundColorProperty );
        set => SetValue( BackgroundColorProperty, value );
    }
}
