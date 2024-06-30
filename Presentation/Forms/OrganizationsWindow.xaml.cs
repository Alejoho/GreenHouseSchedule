using Domain.Processors;
using log4net;
using Presentation.AddEditForms;
using SupportLayer;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for Organizations.xaml
/// </summary>
public partial class OrganizationsWindow : Window
{
    private static readonly ILog _log = LogHelper.GetLogger();
    List<Organization> _organizations;
    List<Municipality> _municipalities;
    List<Province> _provinces;
    OrganizationProcessor _organizationProcessor;
    MunicipalityProcessor _municipalityProcessor;
    Municipality _municipalityModel;

    public OrganizationsWindow()
    {
        InitializeComponent();
        _organizationProcessor = new OrganizationProcessor();
        _municipalityProcessor = new MunicipalityProcessor();
        _municipalityModel = new Municipality();
        LoadData();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnNewOrganization_Click(object sender, RoutedEventArgs e)
    {
        AddEditOrganizationWindow window = new AddEditOrganizationWindow();
        window.ShowDialog();
        RefreshData();
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
        if (dgOrganizations.SelectedItem is Organization organization)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _organizationProcessor.DeleteOrganization(organization.Id);

                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(organization);
                    _log.Info("An Organization record was deleted from the DB");
                    log4net.GlobalContext.Properties["Model"] = "";

                    _organizations.Remove(organization);
                    RefreshData();
                }
                catch (System.InvalidOperationException ex)
                {
                    _log.Error("Attend to delete a related Organization record from the DB", ex);

                    MessageBox.Show("El registro seleccionado no se puede borrar, porque esta relacionado " +
                        "con otros registros.", "Operación Invalida", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void lbltxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filter = lbltxtSearch.TextBox.Text;
        _organizations = _organizationProcessor.GetFilteredOrganizations(filter).ToList();
        dgOrganizations.ItemsSource = null;
        dgOrganizations.ItemsSource = _organizations;
    }

    private void btnAddMunicipality_Click(object sender, RoutedEventArgs e)
    {
        ValidateDataType();

        if (_municipalityProcessor.SaveMunicipality(_municipalityModel) == true)
        {
            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_municipalityModel);
            _log.Info("A Municipality record was saved to the DB");
            log4net.GlobalContext.Properties["Model"] = "";

            RefreshData();
            txtMunicipality.Text = "";
            cmbProvince.SelectedItem = null;
            _municipalityModel = new Municipality();
            btnDeleteMunicipality.IsEnabled = true;
        }
        else
        {
            ShowError();
        }
    }

    private void btnDeleteMunicipality_Click(object sender, RoutedEventArgs e)
    {
        if (dgMunicipalities.SelectedItem is Municipality municipality)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _municipalityProcessor.DeleteMunicipality(municipality.Id);

                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_municipalityModel);
                    _log.Info("A Municipality record was deleted from the DB");
                    log4net.GlobalContext.Properties["Model"] = "";

                    _municipalities.Remove(municipality);
                    RefreshData();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    _log.Error("Attend to delete a related Municipality record from the DB", ex);

                    MessageBox.Show("El registro seleccionado no se puede borrar, porque esta relacionado " +
                        "con otros registros.", "Operación Invalida", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void dgMunicipalities_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

        if (dgMunicipalities.SelectedItem is Municipality municipality)
        {
            _municipalityModel = municipality;
            txtMunicipality.Text = municipality.Name;
            cmbProvince.SelectedItem = _provinces.Where(province => province.Id == municipality.ProvinceId).Single();
            btnDeleteMunicipality.IsEnabled = false;

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(municipality);
            _log.Info("A double click action was made on the lstMunicipalities to edit a Municipality");
            log4net.GlobalContext.Properties["Model"] = "";
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea editar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void dgMunicipalities_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (_municipalityModel.Id != 0)
        {
            _municipalityModel = new Municipality();
            btnDeleteMunicipality.IsEnabled = true;
            txtMunicipality.Text = "";
            cmbProvince.SelectedItem = null;
        }
    }

    private void LoadData()
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

    private void RefreshData()
    {
        if (lbltxtSearch.TextBox.Text == string.Empty)
        {
            _organizations = _organizationProcessor.GetAllOrganizations().ToList();
        }
        else
        {
            string filter = lbltxtSearch.TextBox.Text;
            _organizations = _organizationProcessor.GetFilteredOrganizations(filter).ToList();
        }

        dgOrganizations.ItemsSource = null;
        dgOrganizations.ItemsSource = _organizations;

        _municipalities = _municipalityProcessor.GetAllMunicipalities().ToList();
        dgMunicipalities.ItemsSource = null;
        dgMunicipalities.ItemsSource = _municipalities;
    }

    private void EditOrganization()
    {
        if (dgOrganizations.SelectedItem is Organization organization)
        {
            AddEditOrganizationWindow window = new AddEditOrganizationWindow(organization);
            window.ShowDialog();
            RefreshData();
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea editar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ShowError()
    {
        MessageBox.Show(_municipalityProcessor.Error, "", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ValidateDataType()
    {
        _municipalityModel.Name = txtMunicipality.Text;

        _municipalityModel.ProvinceId = cmbProvince.SelectedItem != null ?
            ((Province)cmbProvince.SelectedItem).Id :
            (byte)0;
    }
}
