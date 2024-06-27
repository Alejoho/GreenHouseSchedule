using log4net;
using Presentation.IRequesters;
using SupportLayer;
using System;
using System.Windows;

namespace Presentation.InputForms;

//CHECK - What is the property show in the taskbar
/// <summary>
/// Interaction logic for SowInputWindow.xaml
/// </summary>
public partial class SowInputWindow : Window
{
    private static readonly ILog _log = LogHelper.GetLogger();
    private ISownOrderLocationChangeRequester _requester;
    public SowInputWindow(ISownOrderLocationChangeRequester requestingWindow)
    {
        InitializeComponent();
        _requester = requestingWindow;
        dtpSowDate.TimePicker.SelectedDate = DateTime.Today;

        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_requester.OrderLocationInProcess);
        _log.Info("The SowInputWindow was opened to sow an OrderLocation");
        log4net.GlobalContext.Properties["Model"] = "";
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
                int endIndex = ex.Message.IndexOf('(');

                endIndex--;

                _log.Warn("It was passed an incorrect argument", ex);

                MessageBox.Show($"{ex.Message.Substring(0, endIndex)}."
                    , "", MessageBoxButton.OK, MessageBoxImage.Warning);

                if (ex.ParamName == "date")
                {
                    this.dtpSowDate.TimePicker.Focus();
                }
                else if (ex.ParamName == "sownSeedTrays")
                {
                    this.lbltxtSownAmount.TextBox.Focus();
                }
            }
            catch (Exception ex)
            {
                _log.Error("There was an error sowing an OrderLocation", ex);

                MessageBox.Show($"{ex.Message}", "", MessageBoxButton.OK, MessageBoxImage.Error);

                this.Close();
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

        if (string.IsNullOrEmpty(lbltxtSownAmount.FieldContent))
        {
            MessageBox.Show("Debe especificar la cantidad de bandejas sembradas.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            lbltxtSownAmount.TextBox.Focus();
            return false;
        }
        else if (short.TryParse(lbltxtSownAmount.FieldContent, out short amountOfSeedlings) == false)
        {
            MessageBox.Show("La cantidad de bandejas sembradas no está en el formato correcto."
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