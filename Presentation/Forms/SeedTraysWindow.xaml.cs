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

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for SeedTrays.xaml
    /// </summary>
    public partial class SeedTraysWindow : Window
    {
        public SeedTraysWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNewSeedTray_Click(object sender, RoutedEventArgs e)
        {

        }

        //NEXT - Create all the logic of the SeedTraysWindows
    }
}
