
using Domain.Processors;
using SupportLayer.Models;
using System;
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
        List<Municipality> _municipalities;
        OrganizationProcessor _organizationProcessor;
        MunicipalityProcessor _municipalityProcessor;

        public OrganizationsWindow()
        {
            InitializeComponent();
            _organizations = new List<Organization>();
            _municipalities = new List<Municipality>();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        //TODO - este boton tiene un textblock dentro de el, el area de hacer click es el del boton mas el de el textblock el cual sobresale del boton. arreglar este detalle.
        private void btnAddMunicipality_Click(object sender, RoutedEventArgs e)
        {
            Municipality model = ValidateDataType();
            if (model.Name != string.Empty)
            {
                if (_municipalityProcessor.SaveMunicipality(model) == true)
                {
                    MessageBox.Show("Registro salvado");
                    this.Close();
                }
                else
                {
                    ShowError();
                }
            }
        }

        //TODO - este boton tiene un textblock dentro de el, el area de hacer click es el del boton mas el de el textblock el cual sobresale del boton. arreglar este detalle.
        private void btnRemoveMunicipality_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadData()
        {
            ProvinceProcessor processor = new ProvinceProcessor();
            cmbProvince.ItemsSource = processor.GetAllProvinces();
            cmbProvince.DisplayMemberPath = "Name";

            _organizations = _organizationProcessor.GetAllOrganizations().ToList();
            dgOrganizations.ItemsSource = _organizations;

            _municipalities = _municipalityProcessor.GetAllMunicipalities().ToList();
            dgMunicipalities.ItemsSource = _municipalities;
        }

        private void RefreshData()
        {
            throw new NotImplementedException();
        }

        private void EditOrganization()
        {
            throw new NotImplementedException();
        }

        private void ShowError()
        {
            throw new NotImplementedException();
        }

        private Municipality ValidateDataType()
        {
            //NEXT - do this method
            throw new NotImplementedException();
        }

        private void PopulateData()
        {
            throw new NotImplementedException();
        }
    }
}
