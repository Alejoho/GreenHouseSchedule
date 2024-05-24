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
        dtpSowDate.TimePicker.SelectedDate = (DateTime?)DateTime.Now;
    }

    private void btnConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateData() == true)
        {
            _requester.SetTheSownOrderLocation(dtpSowDate.SelectedDateOnly, lbltxtSownAmount.IntNumber);
            this.Close();
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
        else if (int.TryParse(lbltxtSownAmount.FieldContent, out int amountOfSeedlings) == false)
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