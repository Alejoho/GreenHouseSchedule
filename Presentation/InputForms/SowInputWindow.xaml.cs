using Presentation.IRequesters;
using System;
using System.Windows;

namespace Presentation.InputForms;

//CHECK - What is the property show in the taskbar
/// <summary>
/// Interaction logic for SowInputWindow.xaml
/// </summary>
public partial class SowInputWindow : Window
{
    private IOrderLocationChangeRequester _requester;
    public SowInputWindow(IOrderLocationChangeRequester requestingWindow)
    {
        InitializeComponent();
        _requester = requestingWindow;
        dtpSowDate.TimePicker.SelectedDate = DateTime.Today;
    }

    private void btnConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateData() == true)
        {
            try
            {
                _requester.SetTheSownOrderLocation(dtpSowDate.SelectedDateOnly, lbltxtSownAmount.ShortNumber);
            this.Close();
        }
            catch (ArgumentException ex)
            {
                //TODO - Make something to delete the "(Parameter 'date')" from the message.
                MessageBox.Show($"{ex.Message}");

                if (ex.ParamName == "date")
                {
                    this.dtpSowDate.TimePicker.Focus();
    }
                else if (ex.ParamName == "sownSeedTrays")
                {
                    this.lbltxtSownAmount.TextBox.Focus();
                }
            }
            catch(Exception ex)
            {
                //LATER - Implement the log.
                MessageBox.Show($"{ex.Message}");
            }
        }
    }

    private bool ValidateData()
    {
        if (dtpSowDate.TimePicker.SelectedDate == null)
        {
            MessageBox.Show("Debe especificar la fecha en la que se sembraron estas bandejas"
                , "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            dtpSowDate.TimePicker.Focus();
            return false;
    }

        if (lbltxtSownAmount.FieldContent == null
            || lbltxtSownAmount.FieldContent == "")
        {
            MessageBox.Show("Debe especificar la cantidad de bandejas sembradas.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            lbltxtSownAmount.TextBox.Focus();
            return false;
        }
        else if (short.TryParse(lbltxtSownAmount.FieldContent, out short amountOfSeedlings) == false)
        {
            MessageBox.Show("La cantidad de bandejas sembradas no esta en el formato correcto."
                , "Cantidad de bandejas sembradas inválida"
                , MessageBoxButton.OK, MessageBoxImage.Warning);
            lbltxtSownAmount.TextBox.Focus();
            return false;
        }

        return true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}