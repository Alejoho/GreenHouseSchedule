using Presentation.IRequesters;
using System;
using System.Windows;

namespace Presentation.InputForms;

/// <summary>
/// Interaction logic for DeliverInputWindow.xaml
/// </summary>
public partial class DeliverInputWindow : Window
{
    private IDeliveredBlockRequester _requester;
    public DeliverInputWindow(IDeliveredBlockRequester requestingWindow)
    {
        InitializeComponent();
        _requester = requestingWindow;
        dtpDeliveryDate.TimePicker.SelectedDate = DateTime.Today;
    }

    private void btnConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateData() == true)
        {
            try
            {
                _requester.SetTheDeliveredBlock(dtpDeliveryDate.SelectedDateOnly, lbltxtDeliveredAmount.ShortNumber);
                this.Close();
            }
            catch (ArgumentException ex)
            {
                int endIndex = ex.Message.IndexOf('(');

                endIndex--;

                MessageBox.Show($"{ex.Message.Substring(0, endIndex)}.");

                if (ex.ParamName == "date")
                {
                    this.dtpDeliveryDate.TimePicker.Focus();
                }
                else if (ex.ParamName == "deliveredSeedTrays")
                {
                    this.lbltxtDeliveredAmount.TextBox.Focus();
                }
            }
            //LATER - Discoment this error handler
            //catch(Exception ex)
            //{
            //    //LATER - Implement the log.
            //    MessageBox.Show($"{ex.Message}");
            //}
        }
    }

    private bool ValidateData()
    {
        if (dtpDeliveryDate.TimePicker.SelectedDate == null)
        {
            MessageBox.Show("Debe especificar la fecha en la que se sembraron estas bandejas"
                , "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            dtpDeliveryDate.TimePicker.Focus();
            return false;
        }

        if (lbltxtDeliveredAmount.FieldContent == null
            || lbltxtDeliveredAmount.FieldContent == "")
        {
            MessageBox.Show("Debe especificar la cantidad de bandejas sembradas.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            lbltxtDeliveredAmount.TextBox.Focus();
            return false;
        }
        else if (short.TryParse(lbltxtDeliveredAmount.FieldContent, out short amountOfSeedlings) == false)
        {
            MessageBox.Show("La cantidad de bandejas sembradas no esta en el formato correcto."
                , "Cantidad de bandejas sembradas inválida"
                , MessageBoxButton.OK, MessageBoxImage.Warning);
            lbltxtDeliveredAmount.TextBox.Focus();
            return false;
        }

        return true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
