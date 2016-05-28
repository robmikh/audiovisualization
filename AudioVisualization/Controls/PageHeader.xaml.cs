using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AudioVisualization.Controls
{
    /// <summary>
    /// The page header control.
    /// </summary>
    public sealed partial class PageHeader : UserControl
    {
        /// <summary>
        /// The header content.
        /// </summary>
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register("HeaderContent", typeof(UIElement), typeof(PageHeader),
                new PropertyMetadata(DependencyProperty.UnsetValue));

        public PageHeader()
        {
            InitializeComponent();

            Loaded += (s, a) =>
            {
                if (AppShell.Current != null)
                {
                    AppShell.Current.TogglePaneButtonRectChanged += Current_TogglePaneButtonSizeChanged;
                    titleBar.Margin = new Thickness(AppShell.Current.TogglePaneButtonRect.Right, 0, 0, 0);
                }
            };
        }

        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        public UIElement HeaderContent
        {
            get { return (UIElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        private void Current_TogglePaneButtonSizeChanged(AppShell sender, Rect e)
        {
            titleBar.Margin = new Thickness(e.Right, 0, 0, 0);
        }
    }
}
