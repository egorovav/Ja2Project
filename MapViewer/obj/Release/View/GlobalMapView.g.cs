﻿#pragma checksum "..\..\..\View\GlobalMapView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "42D4EE795A6636569622D2B9932C64E189DEE6671DCBD4BFA32D50B2F7D98784"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MapViewer {
    
    
    /// <summary>
    /// GlobalMapView
    /// </summary>
    public partial class GlobalMapView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\View\GlobalMapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbLevelNumber;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\View\GlobalMapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas cMap;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\View\GlobalMapView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image iMap;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MapViewer;component/view/globalmapview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\GlobalMapView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\View\GlobalMapView.xaml"
            ((MapViewer.GlobalMapView)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbLevelNumber = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.cMap = ((System.Windows.Controls.Canvas)(target));
            
            #line 19 "..\..\..\View\GlobalMapView.xaml"
            this.cMap.MouseLeave += new System.Windows.Input.MouseEventHandler(this.cMap_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 4:
            this.iMap = ((System.Windows.Controls.Image)(target));
            
            #line 21 "..\..\..\View\GlobalMapView.xaml"
            this.iMap.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.iMap_MouseDown);
            
            #line default
            #line hidden
            
            #line 22 "..\..\..\View\GlobalMapView.xaml"
            this.iMap.MouseMove += new System.Windows.Input.MouseEventHandler(this.iMap_MouseMove);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

