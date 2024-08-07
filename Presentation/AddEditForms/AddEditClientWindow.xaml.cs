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
/// Interaction logic for ClientsWindow.xaml
/// </summary>
public partial class AddEditClientWindow : Window, IOrganizationRequester
{
    private static readonly ILog _log = LogHelper.GetLogger();
    private ClientProcessor _processor;
    private Client _model;
    private List<Organization> _organizations;
    private IClientRequester _requester;

    public AddEditClientWindow()
    {
        InitializeComponent();
        _processor = new ClientProcessor();
        _model = new Client();
        LoadData();

        _log.Info("The AddEditClientWindow was opened to add a new Client");
    }

    public AddEditClientWindow(IClientRequester requestingWindow)
    {
        InitializeComponent();
        _processor = new ClientProcessor();
        _model = new Client();
        _requester = requestingWindow;
        LoadData();

        _log.Info("The AddEditClientWindow was opened by NewOrderWindow to add a Client");
    }

    public AddEditClientWindow(Client model)
    {
        InitializeComponent();
        _processor = new ClientProcessor();
        _model = model;
        LoadData();
        PopulateData();

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model); ;
        _log.Info("The AddEditClientWindow was opened to edit a Client");
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
            if (_processor.SaveClient(_model) == true)
            {
                //MessageBox.Show("Registro salvado");
                _requester?.ClientComplete(_model);

                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
                _log.Info("A Client record was saved to the DB");
                log4net.GlobalContext.Properties["Model"] = "";

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
        _model.NickName = lbltxtNickName.FieldContent ?? "";
        _model.PhoneNumber = lbltxtPhoneNumber.FieldContent ?? "";
        _model.OtherNumber = lbltxtOtherNumber.FieldContent ?? "";

        if (lblcmbbtnOrganization.ComboBox.SelectedItem != null)
        {
            _model.OrganizationId = ((Organization)lblcmbbtnOrganization.ComboBox.SelectedItem).Id;
        }

        return output;
    }

    private void LoadData()
    {
        OrganizationProcessor organizationProcessor = new OrganizationProcessor();

        _organizations = organizationProcessor.GetAllOrganizations().ToList();

        lblcmbbtnOrganization.ComboBox.ItemsSource = _organizations;
        lblcmbbtnOrganization.ComboBox.DisplayMemberPath = "TypeAndOrganizationName";
    }

    private void PopulateData()
    {
        lbltxtName.FieldContent = _model.Name;
        lbltxtNickName.FieldContent = _model.NickName;
        lbltxtPhoneNumber.FieldContent = _model.PhoneNumber;
        lbltxtOtherNumber.FieldContent = _model.OtherNumber;
        lblcmbbtnOrganization.ComboBox.SelectedItem = _organizations.Where(organization =>
        organization.Id == _model.OrganizationId).Single();
    }

    private void lblcmbbtnOrganization_ButtonClick(object sender, RoutedEventArgs e)
    {
        AddEditOrganizationWindow window = new AddEditOrganizationWindow(this);
        window.ShowDialog();
    }

    public void OrganizationComplete()
    {
        LoadData();
        Organization organization = _organizations.OrderByDescending(x => x.Id).First();
        lblcmbbtnOrganization.ComboBox.SelectedItem = organization;
    }
}
