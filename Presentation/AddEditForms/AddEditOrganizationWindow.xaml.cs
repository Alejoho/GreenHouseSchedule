﻿using Domain.Processors;
using log4net;
using Presentation.IRequesters;
using SupportLayer;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.AddEditForms;

/// <summary>
/// Interaction logic for AddEditOrganizationWindow.xaml
/// </summary>
public partial class AddEditOrganizationWindow : Window, IMunicipalityRequester
{
    private static readonly ILog _log = LogHelper.GetLogger();
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
        LoadData();

        _log.Info("The AddEditOrganizationWindow was opened to add a new Organization");
    }

    public AddEditOrganizationWindow(IOrganizationRequester requestingWindow)
    {
        InitializeComponent();
        _processor = new OrganizationProcessor();
        _model = new Organization();
        _requester = requestingWindow;
        LoadData();

        _log.Info("The AddEditOrganizationWindow was opened by AddEditClientWindow to add a new Organization");
    }

    public AddEditOrganizationWindow(Organization model)
    {
        InitializeComponent();
        _processor = new OrganizationProcessor();
        this._model = model;
        LoadData();
        PopulateData();

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
        _log.Info("The AddEditOrganizationWindow was opened to edit an Organization");
        log4net.GlobalContext.Properties["Model"] = "";
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

                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
                _log.Info("An Organization record was saved to the DB");
                log4net.GlobalContext.Properties["Model"] = "";

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
        MessageBox.Show(_processor.Error, "", MessageBoxButton.OK, MessageBoxImage.Warning);
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

    private void lblcmbLocation_ButtonClick(object sender, RoutedEventArgs e)
    {
        AddMunicipalityWindow window = new AddMunicipalityWindow(this);
        window.ShowDialog();
    }

    public void MunicipalityComplete(Municipality model)
    {
        _locations.Add(model);
        lblcmbLocation.ComboBox.ItemsSource = _locations.OrderBy(x => x.Location);
        lblcmbLocation.ComboBox.SelectedItem = model;
    }
}
