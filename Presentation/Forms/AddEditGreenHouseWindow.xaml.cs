using Domain.Processors;
using SupportLayer.Models;
using System.Windows;


namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for GreenHousesWindow.xaml
    /// </summary>
    public partial class AddEditGreenHouseWindow : Window
    {

        private GreenHouseProcessor _processor;
        private GreenHouse _model;

        public AddEditGreenHouseWindow()
        {
            InitializeComponent();
            _processor = new GreenHouseProcessor();
            _model = new GreenHouse();
            _model.Id = 0;
        }

        public AddEditGreenHouseWindow(GreenHouse model)
        {
            InitializeComponent();
            _processor = new GreenHouseProcessor();
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
                if (_processor.SaveGreenHouse(_model) == true)
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
            decimal width = -1;
            decimal length = -1;

            _model.Name = tbtxtName.FieldContent;

            _model.Description = txtDescription.Text;

            if (tbtxtWidth.FieldContent != string.Empty)
            {
                if (decimal.TryParse(tbtxtWidth.FieldContent, out width))
                {
                    _model.Width = width;
                }
                else
                {
                    MessageBox.Show("Ancho inválido");
                    return false;
                }
            }

            if (tbtxtLength.FieldContent != string.Empty)
            {
                if (decimal.TryParse(tbtxtLength.FieldContent, out length))
                {
                    _model.Length = length;
                }
                else
                {
                    MessageBox.Show("Largo inválido");
                    return false;
                }
            }

            if (width != -1 && length != -1)
            {
                _model.GreenHouseArea = width * length;
            }

            if (decimal.TryParse(tbtxtSeedTrayArea.FieldContent, out decimal seedTrayArea))
            {
                _model.SeedTrayArea = seedTrayArea;
            }
            else
            {
                MessageBox.Show("Área de bandejas inválido");
                return false;
            }

            if (byte.TryParse(tbtxtAmountOfBlocks.FieldContent, out byte amountOfBlocks))
            {
                _model.AmountOfBlocks = amountOfBlocks;
            }
            else
            {
                MessageBox.Show("Cantidad de bloques inválido");
                return false;
            }

            _model.Active = chkActive.IsChecked ?? false;

            return true;
        }

        private void PopulateData()
        {
            tbtxtName.FieldContent = _model.Name;
            tbtxtWidth.FieldContent = _model.Width.ToString();
            tbtxtLength.FieldContent = _model.Length.ToString();
            tbtxtSeedTrayArea.FieldContent = _model.SeedTrayArea.ToString();
            tbtxtAmountOfBlocks.FieldContent = _model.AmountOfBlocks.ToString();
            txtDescription.Text = _model.Description;
        }
    }
}
