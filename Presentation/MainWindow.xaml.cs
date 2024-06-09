using Presentation.Forms;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //LATER - Review the startup of all windows. (WindowStartUpLocation, WindowState) 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGreenHouses_Click(object sender, RoutedEventArgs e)
        {
            GreenHousesWindow window = new GreenHousesWindow();
            window.ShowDialog();
        }

        private void btnSeedTrays_Click(object sender, RoutedEventArgs e)
        {
            SeedTraysWindow window = new SeedTraysWindow();
            window.ShowDialog();
        }

        private void btnClients_Click(object sender, RoutedEventArgs e)
        {
            ClientsWindow window = new ClientsWindow();
            window.ShowDialog();
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow window = new ProductsWindow();
            window.ShowDialog();
        }

        private void btnOrganizations_Click(object sender, RoutedEventArgs e)
        {
            OrganizationsWindow window = new OrganizationsWindow();
            window.ShowDialog();
        }

        private void btnOrderList_Click(object sender, RoutedEventArgs e)
        {
            OrderListWindow window = new OrderListWindow();
            window.ShowDialog();
        }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            NewOrderWindow window = new NewOrderWindow();
            window.ShowDialog();
        }

        private void btnDeliveries_Click(object sender, RoutedEventArgs e)
        {
            DeliveryWindow window = new DeliveryWindow();
            window.ShowDialog();
        }

        private void btnSows_Click(object sender, RoutedEventArgs e)
        {
            SowWindow window = new SowWindow();
            window.ShowDialog();
        }

        private void btnOrderDistribution_Click(object sender, RoutedEventArgs e)
        {
            OrderDistributionWindow window = new OrderDistributionWindow();
            window.ShowDialog();
        }

        private void btnUnloads_Click(object sender, RoutedEventArgs e)
        {
            UnloadWindow window = new UnloadWindow();
            window.ShowDialog();
        }
    }
}
