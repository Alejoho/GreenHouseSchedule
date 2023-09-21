using Domain;
using SupportLayer.DatabaseModels;
using System.Windows;


namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for GreenHousesWindow.xaml
    /// </summary>
    public partial class AddEditGreenHouseWindow : Window
    {
        private GreenHouseProcessor processor;
        private Greenhouse model;
        public AddEditGreenHouseWindow()
        {
            InitializeComponent();
            processor = new GreenHouseProcessor();
            model = new Greenhouse();
            model.ID = 0;
        }

        public AddEditGreenHouseWindow(Greenhouse model)
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
            //bool? test = chkActive.IsChecked;
            //return;
            PopulateModel();
            if (processor.SaveGreenHouse(model) == true)
            {
                //TODO - mejorar el mensaje
                MessageBox.Show("Registro salvado");
                this.Close();
            }
            else
            {
                ShowError();
            }
        }

        private void ShowError()
        {
            MessageBox.Show(processor.FirstError);
        }

        private void PopulateModel()
        {
            model.Name = tbtxtName.FieldContent;
            model.Description = txtDescription.Text;

            if (decimal.TryParse(tbtxtWidth.FieldContent, out decimal width))
            {
                model.Width = width;
            }
            else
            {
                MessageBox.Show("Ancho inválido");
                return;
            }

            if (decimal.TryParse(tbtxtLength.FieldContent, out decimal length))
            {
                model.Length = length;
            }
            else
            {
                MessageBox.Show("Largo inválido");
                return;
            }

            if (decimal.TryParse(tbtxtGreenHouseArea.FieldContent, out decimal greenHouseArea))
            {
                model.GreenHouseArea = greenHouseArea;
            }
            else
            {
                MessageBox.Show("Área de la casa de posturas inválido");
                return;
            }

            if (decimal.TryParse(tbtxtSeedTrayArea.FieldContent, out decimal seedTrayArea))
            {
                model.SeedTrayArea = seedTrayArea;
            }
            else
            {
                MessageBox.Show("Área de bandejas inválido");
                return;
            }

            if (byte.TryParse(tbtxtAmountOfBlocks.FieldContent, out byte amountOfBlocks))
            {
                model.SeedTrayArea = amountOfBlocks;
            }
            else
            {
                MessageBox.Show("Área de bandejas inválido");
                return;
            }

            model.Active = chkActive.IsChecked ?? false;

            //model.GreenHouseArea
        }
    }
}
