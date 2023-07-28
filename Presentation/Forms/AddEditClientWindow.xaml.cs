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
    /// Interaction logic for ClientsWindow.xaml
    /// </summary>
    public partial class AddEditClientWindow : Window
    {
        public AddEditClientWindow()
        {
            InitializeComponent();            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(lbtxtName.FieldContent);
            //MessageBox.Show(lbtxtName.TextBox.Text);
            MessageBox.Show(lblcmbbtnOrganization.ComboBox.Text);
            MessageBox.Show(lblcmbbtnOrganization.FieldContent);

        }
    }
}
