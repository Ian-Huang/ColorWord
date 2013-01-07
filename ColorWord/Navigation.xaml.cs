using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace ColorWord
{
    /// <summary>
    /// Navigation.xaml 的互動邏輯
    /// </summary>
    public partial class Navigation : NavigationWindow
    {
        public double width;
        public double height;

        public Navigation()
        {
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(Navigation_SizeChanged);
        }

        void Navigation_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Page page = (Page)this.Content;
            page.Width = e.NewSize.Width;
            page.Height = e.NewSize.Height;
        }

        void ChangeWindowSize(double x, double y)
        {            
            this.Width = x;
            this.Height = y;
        }

        private void navigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.width = this.Width;
            this.height = this.Height;
        }
    }
}
