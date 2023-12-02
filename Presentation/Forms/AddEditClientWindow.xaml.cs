using Domain.Processors;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for ClientsWindow.xaml
/// </summary>
public partial class AddEditClientWindow : Window, IOrganizationRequester
{
    private ClientProcessor _processor;
    private Client _model;
    private List<Organization> _organizations;

    public AddEditClientWindow()
    {
        InitializeComponent();
        _processor = new ClientProcessor();
        _model = new Client();
        //CHECK - If I remove the next statement will the id propertie remain with the same value
        _model.Id = 0;
        LoadData();
    }

    public AddEditClientWindow(Client model)
    {
        InitializeComponent();
        _processor = new ClientProcessor();
        _model = model;
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
            if (_processor.SaveClient(_model) == true)
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

    //LATER - Change the name of this method. Something like PopulateModel fit better.
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
            //CHECK - After I edit a client, the clients window refreshes the field organization
            //by insstanciating again the ClientRepository in the ClientProcessor by calling
            //the method GetAllClient. Evaluate if 
            //is better reasign the Organization property in the model like the next line.
            //_model.Organization = (Organization)lblcmbbtnOrganization.ComboBox.SelectedItem;
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
        //LATER - Evaluate if the nullable properties value should be set "nulo" when display their values"
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
