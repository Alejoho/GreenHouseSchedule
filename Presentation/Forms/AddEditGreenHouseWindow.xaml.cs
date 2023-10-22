using Domain;
using SupportLayer.Models;
using System;
using System.Linq;
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
            LlenarCasillas();
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

            //bool? test = chkActive.IsChecked;
            //return;
            /*
            if (ValidateDataType() == true)
            {
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
            */

            var lista = processor.GetAllGreenHouses();
            Console.WriteLine(lista.Count());
        }

        private void ShowError()
        {
            MessageBox.Show(processor.FirstError);
        }

        private bool ValidateDataType()
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
                return false;
            }

            if (decimal.TryParse(tbtxtLength.FieldContent, out decimal length))
            {
                model.Lenght = length;
            }
            else
            {
                MessageBox.Show("Largo inválido");
                return false;
            }

            //if (decimal.TryParse(tbtxtGreenHouseArea.FieldContent, out decimal greenHouseArea))
            //{
            model.GreenHouseArea = width * length;
            //}
            //else
            //{
            //    MessageBox.Show("Área de la casa de posturas inválido");
            //    return false;
            //}

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

        private void LlenarCasillas()
        {
            tbtxtName.FieldContent = "casa 9";
            txtDescription.Text = "esto es una casa grande";
            tbtxtWidth.FieldContent = "10";
            tbtxtLength.FieldContent = "25";
            tbtxtSeedTrayArea.FieldContent = "195";
            tbtxtAmountOfBlocks.FieldContent = "4";
        }
    }
}
