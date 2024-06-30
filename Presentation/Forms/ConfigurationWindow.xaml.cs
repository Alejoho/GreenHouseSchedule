using Domain;
using Domain.Processors;
using SupportLayer;
using System.Configuration;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for ConfigurationWindow.xaml
/// </summary>
public partial class ConfigurationWindow : Window
{
    //NEWFUNC - Implement this window to change the settings of the app
    ConfigurationProcessor _processor;
    Configurations _model;
    public ConfigurationWindow()
    {
        InitializeComponent();
        _processor = new ConfigurationProcessor();
        LoadData();
    }

    private void LoadData()
    {
        lbltxtRegressionDays.FieldContent = ConfigurationManager
            .AppSettings[ConfigurationNames.RegressionDays] ?? "null";
        lbltxtDailySowingPotential.FieldContent = ConfigurationManager
            .AppSettings[ConfigurationNames.DailySowingPotential] ?? "null";
        lbltxtMinimumLimitOfSowPerDay.FieldContent = ConfigurationManager
            .AppSettings[ConfigurationNames.MinimumLimitOfSowPerDay] ?? "null";
        lbltxtLocationMinimumSeedTray.FieldContent = ConfigurationManager
            .AppSettings[ConfigurationNames.LocationMinimumSeedTray] ?? "null";
        lbltxtSeedlingMultiplier.FieldContent = ConfigurationManager
            .AppSettings[ConfigurationNames.SeedlingMultiplier] ?? "null";
        lbltxtSowShowRange.FieldContent = ConfigurationManager
            .AppSettings[ConfigurationNames.SowShowRange] ?? "null";
        lbltxtDeliveryShowRange.FieldContent = ConfigurationManager
            .AppSettings[ConfigurationNames.DeliveryShowRange] ?? "null";
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnOrganization_Click(object sender, RoutedEventArgs e)
    {
        OrganizationsWindow window = new OrganizationsWindow();
        window.ShowDialog();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateDataType() == true)
        {
            if (_processor.SaveConfigurations(_model) == true)
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
        _model = new Configurations();

        if (int.TryParse(lbltxtRegressionDays.FieldContent,
            out int regressionDays) == true)
        {
            _model.RegressionDays = regressionDays;
        }
        else
        {
            MessageBox.Show("Días de retroceso inválido");
            return false;
        }

        if (int.TryParse(lbltxtDailySowingPotential.FieldContent,
            out int dailySowingPotential) == true)
        {
            _model.DailySowingPotential = dailySowingPotential;
        }
        else
        {
            MessageBox.Show("Potencial de siembra diario inválido");
            return false;
        }

        if (int.TryParse(lbltxtMinimumLimitOfSowPerDay.FieldContent,
            out int minimumLimitOfSowPerDay) == true)
        {
            _model.MinimumLimitOfSowPerDay = minimumLimitOfSowPerDay;
        }
        else
        {
            MessageBox.Show("Siembra diaria mínima inválida");
            return false;
        }

        if (int.TryParse(lbltxtLocationMinimumSeedTray.FieldContent,
            out int locationMinimumSeedTray) == true)
        {
            _model.LocationMinimumSeedTray = locationMinimumSeedTray;
        }
        else
        {
            MessageBox.Show("Bandejas mínimas de una locación inválida");
            return false;
        }

        if (double.TryParse(lbltxtSeedlingMultiplier.FieldContent,
            out double seedlingMultiplier) == true)
        {
            _model.SeedlingMultiplier = seedlingMultiplier;
        }
        else
        {
            MessageBox.Show("Multiplicador de posturas inválido");
            return false;
        }

        if (int.TryParse(lbltxtSowShowRange.FieldContent,
            out int sowShowRange) == true)
        {
            _model.SowShowRange = sowShowRange;
        }
        else
        {
            MessageBox.Show("Rango de muestra de siembra inválido");
            return false;
        }

        if (int.TryParse(lbltxtDeliveryShowRange.FieldContent,
            out int deliveryShowRange) == true)
        {
            _model.DeliveryShowRange = deliveryShowRange;
        }
        else
        {
            MessageBox.Show("Rango de muestra de entregas inválido");
            return false;
        }

        return true;
    }
}
