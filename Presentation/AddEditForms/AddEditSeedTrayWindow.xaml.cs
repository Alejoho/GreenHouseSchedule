using Domain.Processors;
using log4net;
using SupportLayer;
using SupportLayer.Models;
using System.Windows;

namespace Presentation.AddEditForms
{
    /// <summary>
    /// Interaction logic for SeedTraysWindow.xaml
    /// </summary>
    public partial class AddEditSeedTrayWindow : Window
    {
        private static readonly ILog _log = LogHelper.GetLogger();
        private SeedTrayProcessor _processor;
        private SeedTray _model;

        public AddEditSeedTrayWindow()
        {
            InitializeComponent();
            _processor = new SeedTrayProcessor();
            _model = new SeedTray();

            _log.Info("The AddEditSeedTrayWindow was opened to add a new SeedTray");
        }

        public AddEditSeedTrayWindow(SeedTray model)
        {
            InitializeComponent();
            _processor = new SeedTrayProcessor();
            this._model = model;
            PopulateData();

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
            _log.Info("The AddEditSeedTrayWindow was opened to edit a SeedTray");
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
                //NEWFUNC - For another version maybe add an enum with 3 element: Saved, InvalidInput and Error
                //If the returned value is:
                //Saved: se muestra el mensaje de salvado
                //InvalidInput: se muestra un mesaje con el triangulo amarillo
                //Error: se muestra el mensaje con el circulo rojo.
                if (_processor.SaveSeedTray(_model) == true)
                {
                    MessageBox.Show("Registro salvado");

                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_model);
                    _log.Info("A SeedTray record was saved to the DB");
                    log4net.GlobalContext.Properties["Model"] = "";

                    this.Close();
                }
                else
                {
                    ShowError();
                    _model = new SeedTray();
                }
            }
        }

        private void ShowError()
        {
            MessageBox.Show(_processor.Error, "", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool ValidateDataType()
        {
            decimal trayLength = -1;
            decimal trayWidth = -1;

            _model.Name = lbltxtName.FieldContent;

            if (short.TryParse(lbltxtTotalAlveolus.FieldContent, out short totalAlveolus))
            {
                _model.TotalAlveolus = totalAlveolus;
            }
            else
            {
                MessageBox.Show("Total de alvéolos inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (string.IsNullOrEmpty(lbltxtAlveolusLength.FieldContent) == false)
            {
                if (byte.TryParse(lbltxtAlveolusLength.FieldContent, out byte alveolusLength))
                {
                    _model.AlveolusLength = alveolusLength;
                }
                else
                {
                    MessageBox.Show("Alvéolos a lo largo inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(lbltxtAlveolusWidth.FieldContent) == false)
            {
                if (byte.TryParse(lbltxtAlveolusWidth.FieldContent, out byte alveolusWidth))
                {
                    _model.AlveolusWidth = alveolusWidth;
                }
                else
                {
                    MessageBox.Show("Alvéolos a lo ancho inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(lbltxtTrayLength.FieldContent) == false)
            {
                if (decimal.TryParse(lbltxtTrayLength.FieldContent, out trayLength))
                {
                    _model.TrayLength = trayLength;
                }
                else
                {
                    MessageBox.Show("Largo de la bandeja inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(lbltxtTrayWidth.FieldContent) == false)
            {
                if (decimal.TryParse(lbltxtTrayWidth.FieldContent, out trayWidth))
                {
                    _model.TrayWidth = trayWidth;
                }
                else
                {
                    MessageBox.Show("Ancho de la bandeja inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }

            if (trayLength != -1 && trayWidth != -1)
            {
                _model.TrayArea = trayLength * trayWidth;
            }

            if (decimal.TryParse(lbltxtLogicalArea.FieldContent, out decimal logicalTrayArea))
            {
                _model.LogicalTrayArea = logicalTrayArea;
            }
            else
            {
                MessageBox.Show("Área logica de la bandeja inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (short.TryParse(lbltxtTotalAmount.FieldContent, out short totalAmount))
            {
                _model.TotalAmount = totalAmount;
            }
            else
            {
                MessageBox.Show("Cantidad de bandejas inválido", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            _model.Material = lbltxtMaterial.FieldContent;
            _model.Active = chkActive.IsChecked ?? false;
            return true;
        }
        private void PopulateData()
        {
            lbltxtName.FieldContent = _model.Name;
            lbltxtTotalAlveolus.FieldContent = _model.TotalAlveolus.ToString();
            lbltxtAlveolusLength.FieldContent = _model.AlveolusLength.ToString();
            lbltxtAlveolusWidth.FieldContent = _model.AlveolusWidth.ToString();
            lbltxtTrayLength.FieldContent = _model.TrayLength.ToString();
            lbltxtTrayWidth.FieldContent = _model.TrayWidth.ToString();
            lbltxtLogicalArea.FieldContent = _model.LogicalTrayArea.ToString();
            lbltxtTotalAmount.FieldContent = _model.TotalAmount.ToString();
            lbltxtMaterial.FieldContent = _model.Material;
            chkActive.IsChecked = _model.Active;
        }
    }
}
