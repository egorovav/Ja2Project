﻿#pragma checksum "..\..\..\View\ShapeView3D.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4DDD8505BC31C9C99DEF3575FE1FDAD0D83F37889A5C23BAC0BD3903FD60FF75"
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
using _3DTools;


namespace JsdEditor {
    
    
    /// <summary>
    /// ShapeView3D
    /// </summary>
    public partial class ShapeView3D : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 106 "..\..\..\View\ShapeView3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.Model3DGroup mgRoot;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\View\ShapeView3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.Model3DGroup mgShape;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\View\ShapeView3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.AxisAngleRotation3D anrX;
        
        #line default
        #line hidden
        
        
        #line 119 "..\..\..\View\ShapeView3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.AxisAngleRotation3D anrY;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\..\View\ShapeView3D.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Media3D.AxisAngleRotation3D anrZ;
        
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
            System.Uri resourceLocater = new System.Uri("/JsdEditor;component/view/shapeview3d.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\ShapeView3D.xaml"
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
            
            #line 9 "..\..\..\View\ShapeView3D.xaml"
            ((JsdEditor.ShapeView3D)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.UserControl_MouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.mgRoot = ((System.Windows.Media.Media3D.Model3DGroup)(target));
            return;
            case 3:
            this.mgShape = ((System.Windows.Media.Media3D.Model3DGroup)(target));
            return;
            case 4:
            this.anrX = ((System.Windows.Media.Media3D.AxisAngleRotation3D)(target));
            return;
            case 5:
            this.anrY = ((System.Windows.Media.Media3D.AxisAngleRotation3D)(target));
            return;
            case 6:
            this.anrZ = ((System.Windows.Media.Media3D.AxisAngleRotation3D)(target));
            return;
            case 7:
            
            #line 140 "..\..\..\View\ShapeView3D.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Checked += new System.Windows.RoutedEventHandler(this.TransparencyCheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 141 "..\..\..\View\ShapeView3D.xaml"
            ((System.Windows.Controls.CheckBox)(target)).Unchecked += new System.Windows.RoutedEventHandler(this.TransparencyCheckBox_Checked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

