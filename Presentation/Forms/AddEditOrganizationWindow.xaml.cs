using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for AddEditOrganizationWindow.xaml
/// </summary>
public partial class AddEditOrganizationWindow : Window
{
    private OrganizationProcessor _processor;
    private Organization _model;
    private List<TypesOfOrganization> _typesOfOrganizations;
    private List<Municipality> _locations;

    public AddEditOrganizationWindow()
    {
        InitializeComponent();
        _processor = new OrganizationProcessor();
        _model = new Organization();
        _model.Id = 0;
        LoadData();
    }

    public AddEditOrganizationWindow(Organization model)
    {
        InitializeComponent();
        _processor = new OrganizationProcessor();
        this._model = model;
        LoadData();
        PopulateData();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateDataType() == true)
        {
            if (_processor.SaveOrganization(_model) == true)
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

    private void ShowError()
    {
        MessageBox.Show(_processor.Error);
    }

    private bool ValidateDataType()
    {
        bool output = true;
        _model.Name = lbltxtName.FieldContent;

        if (lblcmbType.ComboBox.SelectedItem != null)
        {
            _model.TypeOfOrganizationId = ((TypesOfOrganization)lblcmbType.ComboBox.SelectedItem).Id;
        }

        if (lblcmbLocation.ComboBox.SelectedItem != null)
        {
            _model.MunicipalityId = ((Municipality)lblcmbLocation.ComboBox.SelectedItem).Id;
        }

        return output;
    }

    private void LoadData()
    {
        TypeOfOrganizationProcessor typeOfOrganizationProcessor
            = new TypeOfOrganizationProcessor();

        _typesOfOrganizations = typeOfOrganizationProcessor
            .GetAllTypesOfOrganizations().ToList();

        lblcmbType.ComboBox.ItemsSource = _typesOfOrganizations;
        lblcmbType.ComboBox.DisplayMemberPath = "Name";


        MunicipalityProcessor municipalityProcessor
            = new MunicipalityProcessor();

        _locations = municipalityProcessor.GetAllMunicipalities().ToList();

        lblcmbLocation.ComboBox.ItemsSource = _locations;
        lblcmbLocation.ComboBox.DisplayMemberPath = "Location";
    }
    //NEXT - do this method to be able to update a record
    private void PopulateData()
    {
        throw new NotImplementedException();
    }
}
