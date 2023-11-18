
using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for Organizations.xaml
    /// </summary>
    public partial class OrganizationsWindow : Window
    {
        //NEXT - finish the logic of the organizations window        
        //CHECK - Review the columns of the organization datagrid
        //CHECK - Review the columns of the municipality datagrid
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
            LoadAndRefreshData();
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
        
        //NEXT - do the logic to edit a municipality

        //TODO - este boton tiene un textblock dentro de el, el area de hacer click es el del boton mas el de el textblock el cual sobresale del boton. arreglar este detalle.
        private void btnAddMunicipality_Click(object sender, RoutedEventArgs e)
        {
            Municipality model = ValidateDataType();

            if (_municipalityProcessor.SaveMunicipality(model) == true)
            {
                MessageBox.Show("Registro salvado");
                LoadAndRefreshData();
                txtMunicipality.Text = "";
            }
            else
            {
                ShowError();
            }
        }

        //TODO - este boton tiene un textblock dentro de el, el area de hacer click es el del boton mas el de el textblock el cual sobresale del boton. arreglar este detalle.
        private void btnRemoveMunicipality_Click(object sender, RoutedEventArgs e)
        {
            if (dgMunicipalities.SelectedItem is Municipality municipality)
            {
                if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                    , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _municipalityProcessor.DeleteMunicipality(municipality.Id);
                    _municipalities.Remove(municipality);
                    LoadAndRefreshData();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                    , "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadAndRefreshData()
        {
            ProvinceProcessor processor = new ProvinceProcessor();
            cmbProvince.ItemsSource = processor.GetAllProvinces();
            cmbProvince.DisplayMemberPath = "Name";

            _organizations = _organizationProcessor.GetAllOrganizations().ToList();
            dgOrganizations.ItemsSource = null;
            dgOrganizations.ItemsSource = _organizations;

            _municipalities = _municipalityProcessor.GetAllMunicipalities().ToList();            
            dgMunicipalities.ItemsSource = null;
            dgMunicipalities.ItemsSource = _municipalities;
        }

        private void EditOrganization()
        {
            throw new NotImplementedException();
        }

        private void ShowError()
        {
            MessageBox.Show(_municipalityProcessor.Error);
        }

        private Municipality ValidateDataType()
        {
            Municipality output = new Municipality();

            output.Name = txtMunicipality.Text;

            output.ProvinceId = cmbProvince.SelectedItem != null ?
                ((Province)cmbProvince.SelectedItem).Id :
                (byte)0;

            return output;
        }

        private void PopulateData()
        {
            throw new NotImplementedException();
        }
    }
}
