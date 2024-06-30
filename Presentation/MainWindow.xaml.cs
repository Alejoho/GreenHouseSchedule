using log4net;
using Presentation.Forms;
using SupportLayer;
using System.Windows;

namespace Presentation;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static readonly ILog _log = LogHelper.GetLogger();
    //LATER - Review the startup of all windows. (WindowStartUpLocation, WindowState) 
    //NEWFUNC - Maybe put in all textboxes' tittles the name of the app

    //NEWFUNC - Create 2 custom controls. One for the DataGrid's without RowDetailTemplate and another for the ones
    //with RowDetailTemplate
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnGreenHouses_Click(object sender, RoutedEventArgs e)
    {
        GreenHousesWindow window = new GreenHousesWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnSeedTrays_Click(object sender, RoutedEventArgs e)
    {
        SeedTraysWindow window = new SeedTraysWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnClients_Click(object sender, RoutedEventArgs e)
    {
        ClientsWindow window = new ClientsWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnProducts_Click(object sender, RoutedEventArgs e)
    {
        ProductsWindow window = new ProductsWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnOrganizations_Click(object sender, RoutedEventArgs e)
    {
        OrganizationsWindow window = new OrganizationsWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnOrderList_Click(object sender, RoutedEventArgs e)
    {
        OrderListWindow window = new OrderListWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnNewOrder_Click(object sender, RoutedEventArgs e)
    {
        NewOrderWindow window = new NewOrderWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnDeliveries_Click(object sender, RoutedEventArgs e)
    {
        DeliveryWindow window = new DeliveryWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnSows_Click(object sender, RoutedEventArgs e)
    {
        SowWindow window = new SowWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnOrderDistribution_Click(object sender, RoutedEventArgs e)
    {
        OrderDistributionWindow window = new OrderDistributionWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }

    private void btnUnloads_Click(object sender, RoutedEventArgs e)
    {
        UnloadWindow window = new UnloadWindow();
        _log.Info($"Opened the {window.GetType().Name}");
        window.ShowDialog();
        _log.Info($"Closed the {window.GetType().Name}");
    }
}
