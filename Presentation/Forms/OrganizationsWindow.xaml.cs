
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
        //NEXT - do the logic of the organizations part of this windows        
        //CHECK - Review the columns of the organization datagrid
        //CHECK - Review the columns of the municipality datagrid
        List<Organization> _organizations;
        List<Municipality> _municipalities;
        List<Province> _provinces;
        OrganizationProcessor _organizationProcessor;
        MunicipalityProcessor _municipalityProcessor;
        Municipality _municipalityModel;

        public OrganizationsWindow()
        {
            InitializeComponent();
            _organizations = new List<Organization>();
            _municipalities = new List<Municipality>();
            _provinces = new List<Province>();
            _organizationProcessor = new OrganizationProcessor();
            _municipalityProcessor = new MunicipalityProcessor();
            _municipalityModel = new Municipality();
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

        //LATER - Shearch how to select the edited or added item

        //TODO - este boton tiene un textblock dentro de el, el area de hacer click es el del boton mas el de el textblock el cual sobresale del boton. arreglar este detalle.
        private void btnAddMunicipality_Click(object sender, RoutedEventArgs e)
        {
            ValidateDataType(_municipalityModel);

            if (_municipalityProcessor.SaveMunicipality(_municipalityModel) == true)
            {
                LoadAndRefreshData();
                txtMunicipality.Text = "";
                _municipalityModel = new Municipality();
                btnRemoveMunicipality.IsEnabled = true;
                
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
            _provinces = processor.GetAllProvinces().ToList();
            cmbProvince.ItemsSource = _provinces;
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

        //private Municipality ValidateDataType()
        //{
        //    Municipality output = new Municipality();

        //    output.Name = txtMunicipality.Text;

        //    output.ProvinceId = cmbProvince.SelectedItem != null ?
        //        ((Province)cmbProvince.SelectedItem).Id :
        //        (byte)0;

        //    return output;
        //}

        private void ValidateDataType(Municipality model)
        {
            model.Name = txtMunicipality.Text;

            model.ProvinceId = cmbProvince.SelectedItem != null ?
                ((Province)cmbProvince.SelectedItem).Id :
                (byte)0;
        }

        private void PopulateData()
        {
            throw new NotImplementedException();
        }

        private void dgMunicipalities_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (dgMunicipalities.SelectedItem is Municipality municipality)
            {
                _municipalityModel = municipality;
                txtMunicipality.Text = municipality.Name;
                cmbProvince.SelectedItem = _provinces.Where(province => province.Id == municipality.ProvinceId).Single();
                //cmbProvince.Text = municipality.Name;
                btnRemoveMunicipality.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Debe seleccionar el registro que desea editar.");
            }
        }

        private void dgMunicipalities_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_municipalityModel.Id != 0)
            {
                _municipalityModel = new Municipality();
                btnRemoveMunicipality.IsEnabled = true;
                txtMunicipality.Text = "";
                cmbProvince.SelectedItem = null;
            }
        }
    }
}
