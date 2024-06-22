using Domain.Processors;
using Presentation.IRequesters;
using SupportLayer.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.InputForms
{
    /// <summary>
    /// Interaction logic for PlaceInputWindow.xaml
    /// </summary>
    public partial class PlaceInputWindow : Window
    {
        private IPlacedOrderLocationChangeRequester _requester;

        public PlaceInputWindow(IPlacedOrderLocationChangeRequester requestingWindow)
        {
            InitializeComponent();
            _requester = requestingWindow;
            LoadData();
        }

        private void LoadData()
        {
            GreenHouseProcessor processor = new GreenHouseProcessor();
            var greenHouses = processor.GetActiveGreenHouses();

            lblcmbGreenHouse.ComboBox.ItemsSource = greenHouses;
            lblcmbGreenHouse.ComboBox.DisplayMemberPath = "Name";
        }

        private void lblcmbGreenHouse_Selection_Changed(object sender, SelectionChangedEventArgs e)
        {
            int amountOfBlocks = ((GreenHouse)lblcmbGreenHouse.ComboBox.SelectedItem).AmountOfBlocks;

            lblcmbBlock.ComboBox.Items.Clear();

            for (int i = 1; i <= amountOfBlocks; i++)
            {
                lblcmbBlock.ComboBox.Items.Add(i);
            }
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData() == true)
            {
                try
                {
                    byte greenHouseId = ((GreenHouse)lblcmbGreenHouse.ComboBox.SelectedItem).Id;
                    byte blockNumber = Convert.ToByte(lblcmbBlock.ComboBox.SelectedItem);
                    short placedSeedTrays = short.Parse(lbltxtPlacedAmount.FieldContent);

                    _requester.SetThePlacedOrderLocation(greenHouseId, blockNumber, placedSeedTrays);
                    this.Close();
                }
                catch (ArgumentException ex)
                {
                    int endIndex = ex.Message.IndexOf('(');

                    endIndex--;

                    MessageBox.Show($"{ex.Message.Substring(0, endIndex)}.");

                    if (ex.ParamName == "something")
                    {
                        lbltxtPlacedAmount.TextBox.Focus();
                    }
                }
                //catch (Exception ex)
                //{
                //    //NEXT - Implement the log.
                //    MessageBox.Show($"{ex.Message}");
                //}
            }
        }

        private bool ValidateData()
        {
            if (lblcmbGreenHouse.ComboBox.SelectedItem == null)
            {
                MessageBox.Show("Debe especificar la casa en la que se ubicaron estas bandejas"
                    , "Dato faltante"
                    , MessageBoxButton.OK, MessageBoxImage.Information);
                lblcmbGreenHouse.ComboBox.Focus();
                return false;
            }

            if (lblcmbBlock.ComboBox.SelectedItem == null)
            {
                MessageBox.Show("Debe especificar el bloque en el que se ubicaron estas bandejas"
                    , "Dato faltante"
                    , MessageBoxButton.OK, MessageBoxImage.Information);
                lblcmbBlock.ComboBox.Focus();
                return false;
            }

            if (lbltxtPlacedAmount.FieldContent == null
                || lbltxtPlacedAmount.FieldContent == "")
            {
                MessageBox.Show("Debe especificar la cantidad de bandejas ubicadas.", "Dato faltante"
                    , MessageBoxButton.OK, MessageBoxImage.Information);
                lbltxtPlacedAmount.TextBox.Focus();
                return false;
            }
            else if (short.TryParse(lbltxtPlacedAmount.FieldContent, out short amountOfSeedlings) == false)
            {
                MessageBox.Show("La cantidad de bandejas ubicadas no esta en el formato correcto."
                    , "Cantidad de bandejas ubicadas inválida"
                    , MessageBoxButton.OK, MessageBoxImage.Warning);
                lbltxtPlacedAmount.TextBox.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
