using Domain.Processors;
using Presentation.IRequesters;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.Forms;

//NEXT - To all the logic of this window 

//NEXT - Add to the orderlocation table in the db a new field to store the unload date.
/// <summary>
/// Interaction logic for UnloadWindow.xaml
/// </summary>
public partial class UnloadWindow : Window, IPlacedOrderLocationChangeRequester
{
    private ObservableCollection<Order> _orders;
    private OrderProcessor _orderProcessor;
    private OrderLocationProcessor _orderLocationProcessor;
    private OrderLocation _orderLocationInProcess;
    private DataGrid _activeOrderLocationDataGrid;

    public UnloadWindow()
    {
        InitializeComponent();
        _orderProcessor = new OrderProcessor();
        _orderLocationProcessor = new OrderLocationProcessor();
        LoadData();
    }

    private void LoadData()
    {
        _orders = new ObservableCollection<Order>(_orderProcessor.GetNextOrdersToUnload());

        foreach (Order order in _orders)
        {
            var orderLocationsToDisplay = order.OrderLocations.Where(x => x.GreenHouseId == 0);
            order.OrderLocationsView = new ObservableCollection<OrderLocation>(orderLocationsToDisplay);
        }

        dgSowList.DataContext = this;
        dgSowList.ItemsSource = _orders;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnUnload_Click(object sender, RoutedEventArgs e)
    {
        CallOrderLocationPlaceSetter();
    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        CallOrderLocationPlaceSetter();
    }

    private void CallOrderLocationPlaceSetter()
    {
        //NEXT - Create the window to place an orderlocation
    }

    private void btnRowDetail_Click(object sender, RoutedEventArgs e)
    {
        var row = DataGridRow.GetRowContainingElement((Button)sender);

        row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ?
        Visibility.Collapsed : Visibility.Visible;
    }

    private void dgOrderLocationChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is DataGrid datagrid)
        {
            _orderLocationInProcess = (OrderLocation)datagrid.SelectedItem;
            _activeOrderLocationDataGrid = datagrid;
        }
    }

    public void SetThePlacedOrderLocation(byte greenHouse, byte block, short sownSeedTrays)
    {
        //NEXT - Make this method. this is the method which is called from the input window

        _orderLocationProcessor.SavePlacedOrderLocationChange(_orderLocationInProcess, greenHouse, block, sownSeedTrays);

        RefreshTheDataGrids();
    }

    private void RefreshTheDataGrids()
    {
        if (_orderLocationInProcess.Order.Complete == true)
        {
            _orders.Remove(_orderLocationInProcess.Order);
        }
        else if (_orderLocationInProcess.RealSowDate != null)
        {
            Order order = _orderLocationInProcess.Order;
            order.OrderLocationsView.Remove(_orderLocationInProcess);
            order.UIColorsUpdateTrigger++;
        }
        else
        {
            _activeOrderLocationDataGrid.Items.Refresh();
        }
    }

    public OrderLocation OrderLocationInProcess { get => _orderLocationInProcess; }
}
