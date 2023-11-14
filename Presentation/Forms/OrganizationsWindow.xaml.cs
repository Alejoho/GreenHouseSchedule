
using Domain.Processors;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for Organizations.xaml
    /// </summary>
    public partial class OrganizationsWindow : Window
    {
        //NEXT - finish the logic of the organizations window
        //NEXT - Review the columns of the organization datagrid
        //NEXT - Prepare the columns of the municipality datagrid
        List<Organization> _organizations;
        OrganizationProcessor _organizationProcessor;
        MunicipalityProcessor _municipalityProcessor;

        public OrganizationsWindow()
        {
            InitializeComponent();
            _organizations = new List<Organization>();
            _organizationProcessor = new OrganizationProcessor();
            _municipalityProcessor = new MunicipalityProcessor();
            LoadData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNewOrganization_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEditOrganization_Click(object sender, RoutedEventArgs e)
        {
            EditOrganization();
        }

        private void dgOrganizations_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditOrganization();
        }

        private void btnDeleteOrganization_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadData()
        {
            ProvinceProcessor processor = new ProvinceProcessor();
            cmbProvince.ItemsSource = processor.GetAllProvinces().ToList();
            cmbProvince.DisplayMemberPath = "Name";

            _organizations = _organizationProcessor.GetAllOrganizations().ToList();
            dgOrganizations.ItemsSource = _organizations;


        }

        private void RefreshData()
        {

        }

        private void EditOrganization()
        {

        }
    }
}
