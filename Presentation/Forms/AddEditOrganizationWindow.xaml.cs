using Domain.Processors;
using Presentation.IRequesters;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for AddEditOrganizationWindow.xaml
/// </summary>
public partial class AddEditOrganizationWindow : Window
{
    //TODO - Add to this window the funcionality to add a new location in window
    private OrganizationProcessor _processor;
    private Organization _model;
    private List<TypesOfOrganization> _typesOfOrganizations;
    private List<Municipality> _locations;
    IOrganizationRequester _requester;

    public AddEditOrganizationWindow()
    {
        InitializeComponent();
        _processor = new OrganizationProcessor();
        _model = new Organization();
        _model.Id = 0;
        LoadData();
    }

    public AddEditOrganizationWindow(IOrganizationRequester requestingWindow)
    {
        InitializeComponent();
        _processor = new OrganizationProcessor();
        _model = new Organization();
        _model.Id = 0;
        _requester = requestingWindow;
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
                //MessageBox.Show("Registro salvado");
                _requester?.OrganizationComplete();
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

    private void PopulateData()
    {
        lbltxtName.FieldContent = _model.Name;
        lblcmbType.ComboBox.SelectedItem = _typesOfOrganizations
            .Where(type => type.Id == _model.TypeOfOrganizationId).Single();
        lblcmbLocation.ComboBox.SelectedItem = _locations
            .Where(x => x.Id == _model.MunicipalityId).Single();
    }
}
