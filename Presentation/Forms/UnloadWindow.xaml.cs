using Domain.Processors;
using log4net;
using Presentation.InputForms;
using Presentation.IRequesters;
using SupportLayer;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for UnloadWindow.xaml
/// </summary>
public partial class UnloadWindow : Window, IPlacedOrderLocationChangeRequester
{
    private static readonly ILog _log = LogHelper.GetLogger();
    private ObservableCollection<Order> _orders;
    private OrderLocationProcessor _orderLocationProcessor;
    private OrderLocation _orderLocationInProcess;
    private DataGrid _activeOrderLocationDataGrid;

    public UnloadWindow()
    {
        InitializeComponent();
        _orderLocationProcessor = new OrderLocationProcessor();
        LoadData();
    }

    private void LoadData()
    {
        OrderProcessor orderProcessor = new OrderProcessor();

        _orders = new ObservableCollection<Order>(orderProcessor.GetNextOrdersToUnload());

        foreach (Order order in _orders)
        {
            var orderLocationsToDisplay = order.OrderLocations.Where(x => x.RealSowDate != null && x.GreenHouseId == 0);
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
        if (_orderLocationInProcess == null)
        {
            MessageBox.Show("Debe seleccionar la locación que desea desestibar"
                , "", MessageBoxButton.OK, MessageBoxImage.Information);

            return;
        }

        PlaceInputWindow window = new PlaceInputWindow(this);
        window.ShowDialog();
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

    public void SetThePlacedOrderLocation(byte greenHouse, byte block, short placedSeedTrays)
    {
        _orderLocationProcessor.SavePlacedOrderLocationChange(_orderLocationInProcess, greenHouse, block, placedSeedTrays);

        _log.Info("Completed the actual placing");

        RefreshTheDataGrids();
    }

    private void RefreshTheDataGrids()
    {
        Order order = _orderLocationInProcess.Order;

        if (_orderLocationInProcess.GreenHouseId > 0)
        {
            order.OrderLocationsView.Remove(_orderLocationInProcess);
        }

        if (order.OrderLocationsView.Count == 0)
        {
            _orders.Remove(order);
        }

        _activeOrderLocationDataGrid.Items.Refresh();
    }

    public OrderLocation OrderLocationInProcess { get => _orderLocationInProcess; }
}
