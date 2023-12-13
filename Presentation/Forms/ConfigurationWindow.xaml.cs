using SupportLayer;
using System.Configuration;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for ConfigurationWindow.xaml
/// </summary>
public partial class ConfigurationWindow : Window
{
    public ConfigurationWindow()
    {
        InitializeComponent();
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

    //LATER - In general in the windows with datagrids there is a button named "atras"
    //but in code it's named btnCanel. Changed its name to btnBack.
    private void btnBack_Click(object sender, RoutedEventArgs e)
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
        //NEXT - Do the logic to save the configurations
        if (ValidateDataType() == true)
        {
            ConfigurationManager.AppSettings[ConfigurationNames.RegressionDays] =
                lbltxtRegressionDays.FieldContent;
            ConfigurationManager.AppSettings[ConfigurationNames.DailySowingPotential] =
                lbltxtDailySowingPotential.FieldContent;
            ConfigurationManager.AppSettings[ConfigurationNames.MinimumLimitOfSowPerDay] =
                lbltxtMinimumLimitOfSowPerDay.FieldContent;
            ConfigurationManager.AppSettings[ConfigurationNames.LocationMinimumSeedTray] =
                lbltxtLocationMinimumSeedTray.FieldContent;
            ConfigurationManager.AppSettings[ConfigurationNames.SeedlingMultiplier] =
                lbltxtSeedlingMultiplier.FieldContent;
            ConfigurationManager.AppSettings[ConfigurationNames.SowShowRange] =
                lbltxtSowShowRange.FieldContent;
            ConfigurationManager.AppSettings[ConfigurationNames.DeliveryShowRange] =
                lbltxtDeliveryShowRange.FieldContent;            
            this.Close();
        }
        else
        {
            MessageBox.Show("Algunas de las configuraciones es invalida. Revíselas");
        }

    }

    private bool ValidateDataType()
    {
        bool output = true;

        output = int.TryParse(lbltxtRegressionDays.FieldContent, out int C1) == false ?
            false : output;

        output = int.TryParse(lbltxtDailySowingPotential.FieldContent, out int C2) == false ?
            false : output;

        output = int.TryParse(lbltxtMinimumLimitOfSowPerDay.FieldContent, out int C3) == false ?
            false : output;

        output = int.TryParse(lbltxtLocationMinimumSeedTray.FieldContent, out int C4) == false ?
            false : output;

        output = double.TryParse(lbltxtSeedlingMultiplier.FieldContent, out double C5) == false ?
            false : output;

        output = int.TryParse(lbltxtSowShowRange.FieldContent, out int C6) == false ?
            false : output;

        output = int.TryParse(lbltxtDeliveryShowRange.FieldContent, out int C7) == false ?
            false : output;

        return output;
    }
}
