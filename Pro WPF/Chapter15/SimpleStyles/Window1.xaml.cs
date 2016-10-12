using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleStyles
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        void OpenWindow(object sender, RoutedEventArgs args)
        {
            new TestWindow().Visibility = Visibility.Visible;
        }

        void OpenNavWindow(object sender, RoutedEventArgs args)
        {
            new TestNavigationWindow().Visibility = Visibility.Visible;
        }
    }
}
