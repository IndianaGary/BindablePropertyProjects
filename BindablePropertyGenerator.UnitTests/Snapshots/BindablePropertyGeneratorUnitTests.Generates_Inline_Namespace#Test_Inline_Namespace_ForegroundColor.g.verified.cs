﻿//HintName: Test_Inline_Namespace_ForegroundColor.g.cs
//  <auto-generated - BindablePropertyGenerator Version: 0.6.7.0-beta />

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UnitTest
{
    public partial class Test_Inline_Namespace
    {
        public static readonly BindableProperty ForegroundColorProperty
               = BindableProperty.Create( nameof(ForegroundColor),
                                          typeof(Color),
                                          typeof(Test_Inline_Namespace)
                                        );
        public Color ForegroundColor
        {
            get => (Color)GetValue( ForegroundColorProperty );
            set => SetValue( ForegroundColorProperty, value );
        }
    }
}
