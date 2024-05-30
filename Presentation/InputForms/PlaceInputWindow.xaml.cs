using Presentation.IRequesters;
using System;
using System.Windows;

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
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData() == true)
            {
                try
                {
                    _requester.SetThePlacedOrderLocation();
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
                //    //LATER - Implement the log.
                //    MessageBox.Show($"{ex.Message}");
                //}
            }
        }

        private bool ValidateData()
        {
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
                MessageBox.Show("La cantidad de bandejas sembradas no esta en el formato correcto."
                    , "Cantidad de bandejas sembradas inválida"
                    , MessageBoxButton.OK, MessageBoxImage.Warning);
                lbltxtPlacedAmount.TextBox.Focus();
                return false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
