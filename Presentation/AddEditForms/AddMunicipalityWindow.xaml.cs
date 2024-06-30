using Domain.Processors;
using log4net;
using Presentation.IRequesters;
using SupportLayer;
using SupportLayer.Models;
using System.Windows;

namespace Presentation.AddEditForms
{
    /// <summary>
    /// Interaction logic for AddMunicipalityWindow.xaml
    /// </summary>
    public partial class AddMunicipalityWindow : Window
    {
        private static readonly ILog _log = LogHelper.GetLogger();
        private MunicipalityProcessor _processor;
        private Municipality _model;
        IMunicipalityRequester _requester;

        public AddMunicipalityWindow(IMunicipalityRequester requestingWindow)
        {
            InitializeComponent();
            _processor = new MunicipalityProcessor();
            _model = new Municipality();
            _requester = requestingWindow;
            LoadData();

            _log.Info("The AddMunicipalityWindow was opened by AddEditOrganizationWindow to add a new Municipality");
        }

        private void LoadData()
        {
            ProvinceProcessor provinceProcessor = new ProvinceProcessor();
            lblcmbbtnProvince.ComboBox.ItemsSource = provinceProcessor.GetAllProvinces();
            lblcmbbtnProvince.ComboBox.DisplayMemberPath = "Name";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateDataType() == true)
            {
                if (_processor.SaveMunicipality(_model) == true)
                {
                    MessageBox.Show("Registro salvado");

                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
                    _log.Info("A Municipality record was saved to the DB");
                    log4net.GlobalContext.Properties["Model"] = "";

                    _model.Province = (Province)lblcmbbtnProvince.ComboBox.SelectedItem;

                    _requester.MunicipalityComplete(_model);
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

            _model.Name = lbltxtName.TextBox.Text;

            if (lblcmbbtnProvince.ComboBox.SelectedItem != null)
            {
                _model.ProvinceId = ((Province)lblcmbbtnProvince.ComboBox.SelectedItem).Id;
            }

            return output;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
