using Domain.Processors;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms
{
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
            LoadData();
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
            lblcmbLocation.ComboBox.DisplayMemberPath= "Location";
        }
    }
}
