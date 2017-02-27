using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DialogTimeStartStopWpf
{
    /// <summary>
    /// Interaction logic for SelectTimeWindow.xaml
    /// </summary>
    public partial class SelectTimeWindow : Window
    {
        private SelectTimeViewModel _vm;

        public SelectTimeWindow(SelectTimeViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = vm;
        }

        private void StornierenButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.IsValid())
            {
                DialogResult = true;
                this.Close();
            }
        }

        private void AbbrechenButton_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
