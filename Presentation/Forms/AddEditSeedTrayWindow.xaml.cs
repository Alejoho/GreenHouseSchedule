using Domain.Processors;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SupportLayer.Models;
using System.Windows;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for SeedTraysWindow.xaml
    /// </summary>
    public partial class AddEditSeedTrayWindow : Window
    {
        private SeedTrayProcessor _processor;
        private SeedTray _model;

        public AddEditSeedTrayWindow(byte nextPreferenceValue)
        {
            InitializeComponent();
            _processor = new SeedTrayProcessor();
            _model = new SeedTray();
            _model.Id = 0;
            _model.Preference = nextPreferenceValue;
            LlenarCasillas();
        }

        public AddEditSeedTrayWindow(SeedTray model)
        {
            InitializeComponent();
            _processor = new SeedTrayProcessor();
            this._model = model;
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
                if (_processor.SaveSeedTray(_model) == true)
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
            decimal trayLength = -1;
            decimal trayWidth = -1;
            
            _model.Name = lbltxtName.FieldContent;

            if (short.TryParse(lbltxtTotalAlveolus.FieldContent, out short totalAlveolus))
            {
                _model.TotalAlveolus = totalAlveolus;
            }
            else
            {
                MessageBox.Show("Total de alvéolos inválido");
                return false;
            }

            if (lbltxtAlveolusLength.FieldContent != string.Empty)
            {
                if (byte.TryParse(lbltxtAlveolusLength.FieldContent, out byte alveolusLength))
                {
                    _model.AlveolusLength = alveolusLength;
                }
                else
                {
                    MessageBox.Show("Alvéolos a lo largo inválido");
                    return false;
                }
            }

            if (lbltxtAlveolusWidth.FieldContent != string.Empty)
            {
                if (byte.TryParse(lbltxtAlveolusWidth.FieldContent, out byte alveolusWidth))
                {
                    _model.AlveolusWidth = alveolusWidth;
                }
                else
                {
                    MessageBox.Show("Alvéolos a lo ancho inválido");
                    return false;
                }
            }

            if (lbltxtTrayLength.FieldContent != string.Empty)
            {
                if (decimal.TryParse(lbltxtTrayLength.FieldContent, out trayLength))
                {
                    _model.TrayLength = trayLength;
                }
                else
                {
                    MessageBox.Show("Largo de la bandeja inválido");
                    return false;
                }
            }

            if (lbltxtTrayWidth.FieldContent != string.Empty)
            {
                if (decimal.TryParse(lbltxtTrayWidth.FieldContent, out trayWidth))
                {
                    _model.TrayWidth = trayWidth;
                }
                else
                {
                    MessageBox.Show("Ancho de la bandeja inválido");
                    return false;
                }
            }

            if (trayLength!=-1 && trayWidth !=-1)
            {
                _model.TrayArea = trayLength * trayWidth;
            }

            if (decimal.TryParse(lbltxtLogicalArea.FieldContent, out decimal logicalTrayArea))
            {
                _model.LogicalTrayArea = logicalTrayArea;
            }
            else
            {
                MessageBox.Show("Área logica de la bandeja inválido");
                return false;
            }

            if (short.TryParse(lbltxtTotalAmount.FieldContent, out short totalAmount))
            {
                _model.TotalAmount = totalAmount;
            }
            else
            {
                MessageBox.Show("Cantidad de bandejas inválido");
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
            //CHECK - if this property was null would it thrown an error
            lbltxtAlveolusLength.FieldContent = _model.AlveolusLength.ToString();
            lbltxtAlveolusWidth.FieldContent = _model.AlveolusWidth.ToString();
            lbltxtTrayLength.FieldContent = _model.TrayLength.ToString();
            lbltxtTrayWidth.FieldContent = _model.TrayWidth.ToString();
            lbltxtLogicalArea.FieldContent = _model.LogicalTrayArea.ToString();
            lbltxtTotalAmount.FieldContent = _model.TotalAmount.ToString();
            lbltxtMaterial.FieldContent = _model.Material;
            chkActive.IsChecked = _model.Active;
        }
        private void LlenarCasillas()
        {
            lbltxtName.FieldContent = "300 plásticas";
            lbltxtTotalAlveolus.FieldContent = "300";
            lbltxtAlveolusLength.FieldContent = "30";
            lbltxtAlveolusWidth.FieldContent = "10";
            lbltxtTrayLength.FieldContent = "1.2";
            lbltxtTrayWidth.FieldContent = "0.7";
            lbltxtLogicalArea.FieldContent = "0.95";
            lbltxtTotalAmount.FieldContent = "2575";
            lbltxtMaterial.FieldContent = "plástico";            
        }
    }
}
