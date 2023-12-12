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

    }
}
