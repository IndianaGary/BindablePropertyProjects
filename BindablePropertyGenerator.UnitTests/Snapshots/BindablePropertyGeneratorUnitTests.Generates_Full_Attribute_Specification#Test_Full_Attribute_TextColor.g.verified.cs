﻿//HintName: Test_Full_Attribute_TextColor.g.cs
//  <auto-generated - BindablePropertyGenerator Version: 0.6.7.0-beta />

using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UnitTest
{
    public partial class Test_Full_Attribute
    {
        public static readonly BindableProperty TextColorProperty
               = BindableProperty.Create( nameof(TextColor),
                                          typeof(Color),
                                          typeof(Test_Full_Attribute)
                                        );
        public Color TextColor
        {
            get => (Color)GetValue( TextColorProperty );
            set => SetValue( TextColorProperty, value );
        }
    }
}
