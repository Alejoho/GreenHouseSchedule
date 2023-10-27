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

        private GreenHouseProcessor processor;
        private GreenHouse model;

        public AddEditGreenHouseWindow()
        {
            InitializeComponent();
            processor = new GreenHouseProcessor();
            model = new GreenHouse();
            model.Id = 0;
            PopulateData();
        }

        public AddEditGreenHouseWindow(GreenHouse model)
        {
            InitializeComponent();
            processor = new GreenHouseProcessor();
            this.model = model;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateDataType() == true)
            {
                if (processor.SaveGreenHouse(model) == true)
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
            MessageBox.Show(processor.Error);
        }

        private bool ValidateDataType()
        {
            decimal width = -1;
            decimal length = -1;

            model.Name = tbtxtName.FieldContent;

            model.Description = txtDescription.Text;

            if (tbtxtWidth.FieldContent != string.Empty)
            {
                if (decimal.TryParse(tbtxtWidth.FieldContent, out width))
                {
                    model.Width = width;
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
                    model.Length = length;
                }
                else
                {
                    MessageBox.Show("Largo inválido");
                    return false;
                }
            }

            if (width != -1 && length != -1)
            {
                model.GreenHouseArea = width * length;
            }

            if (decimal.TryParse(tbtxtSeedTrayArea.FieldContent, out decimal seedTrayArea))
            {
                model.SeedTrayArea = seedTrayArea;
            }
            else
            {
                MessageBox.Show("Área de bandejas inválido");
                return false;
            }

            if (byte.TryParse(tbtxtAmountOfBlocks.FieldContent, out byte amountOfBlocks))
            {
                model.AmountOfBlocks = amountOfBlocks;
            }
            else
            {
                MessageBox.Show("Cantidad de bloques inválido");
                return false;
            }

            model.Active = chkActive.IsChecked ?? false;

            return true;
        }

        private void PopulateData()
        {
            //NEXT - Check how works the saving of a null width in the database 
            //because i have in the database a constraint, so the width 
            //have to be greater than 0 and less than 50 
            tbtxtName.FieldContent = model.Name;
            tbtxtWidth.FieldContent = model.Width.ToString();
            tbtxtLength.FieldContent = model.Length.ToString();
            tbtxtSeedTrayArea.FieldContent = model.SeedTrayArea.ToString();
            tbtxtAmountOfBlocks.FieldContent = model.AmountOfBlocks.ToString();
            txtDescription.Text = model.Description;
        }
    }
}
