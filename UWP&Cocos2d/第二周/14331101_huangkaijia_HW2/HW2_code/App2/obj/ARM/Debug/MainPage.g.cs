﻿#pragma checksum "C:\Users\cajet\Desktop\14331101_huangkaijia_HW2\HW2_code\App2\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "310843ACEFC6C76DCE7705CC572ED346"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Todos
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.DeleteAppBarButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 12 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.DeleteAppBarButton).Click += this.AddAppBarButton_Click;
                    #line default
                }
                break;
            case 2:
                {
                    this.textBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3:
                {
                    global::Windows.UI.Xaml.Controls.CheckBox element3 = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 57 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)element3).Checked += this.CheckBox_Checked_two;
                    #line 57 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)element3).Unchecked += this.CheckBox_Unchecked_two;
                    #line default
                }
                break;
            case 4:
                {
                    this.Line2 = (global::Windows.UI.Xaml.Shapes.Line)(target);
                }
                break;
            case 5:
                {
                    global::Windows.UI.Xaml.Controls.CheckBox element5 = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 36 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)element5).Checked += this.CheckBox_Checked_one;
                    #line 36 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)element5).Unchecked += this.CheckBox_Unchecked_one;
                    #line default
                }
                break;
            case 6:
                {
                    this.Line1 = (global::Windows.UI.Xaml.Shapes.Line)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

