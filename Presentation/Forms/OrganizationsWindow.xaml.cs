
using Domain.Processors;
using SupportLayer.Models;
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
    /// Interaction logic for Organizations.xaml
    /// </summary>
    public partial class OrganizationsWindow : Window
    {
        //NEXT - do the logic of the organizations window
        public OrganizationsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            ProvinceProcessor processor = new ProvinceProcessor();
            cmbProvince.ItemsSource = processor.GetAllProvinces().ToList();
            cmbProvince.DisplayMemberPath = "Name";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
