﻿

#pragma checksum "E:\Windows 8编程\Dodge WorkingOn\OpenXLivePages\Pages\AchievementsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BB5C66C37912354F91D180949B8230DF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenXLivePages
{
    partial class AchievementsPage : global::OpenXLivePages.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 91 "..\..\Pages\AchievementsPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.Filter_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 76 "..\..\Pages\AchievementsPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.resultsGridView_ItemClick;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 59 "..\..\Pages\AchievementsPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.Filter_Checked;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 40 "..\..\Pages\AchievementsPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.GoBack;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


