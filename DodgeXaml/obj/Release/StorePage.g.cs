﻿

#pragma checksum "E:\Windows 8编程\Dodge WorkingOn\DodgeXaml\StorePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0A7830E4C918C1002C4A8E3E86B25348"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DodgeXaml
{
    partial class StorePage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 11 "..\..\StorePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BtnBack_OnClick;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 12 "..\..\StorePage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerPressed += this.ImgLighting_OnPointerPressed;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 23 "..\..\StorePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BtnTest_OnClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


