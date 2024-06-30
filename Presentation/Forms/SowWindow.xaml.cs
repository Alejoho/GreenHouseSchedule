using Domain.Processors;
using Presentation.InputForms;
using Presentation.IRequesters;
using SupportLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.Forms;

//NEWFUNC - made something for when i have several dgorderlocation open and i change from one to another
//selecting an orderlocation que la seleccion del anterior se ponga a null.

/// <summary>
/// Interaction logic for SowWindow.xaml
/// </summary>
public partial class SowWindow : Window, ISownOrderLocationChangeRequester
{
    private ObservableCollection<Order> _orders;
    private OrderProcessor _orderProcessor;
    private OrderLocationProcessor _orderLocationProcessor;
    private OrderLocation _orderLocationInProcess;
    private DataGrid _activeOrderLocationDataGrid;

    public SowWindow()
    {
        InitializeComponent();
        _orderProcessor = new OrderProcessor();
        _orderLocationProcessor = new OrderLocationProcessor();
        LoadData();
    }

    private void LoadData()
    {
        _orders = new ObservableCollection<Order>(_orderProcessor.GetNextOrdersToSow());

        foreach (Order order in _orders)
        {
            var orderLocationsToDisplay = order.OrderLocations.Where(x => x.RealSowDate == null);
            order.OrderLocationsView = new ObservableCollection<OrderLocation>(orderLocationsToDisplay);
        }

        dgSowList.DataContext = this;
        dgSowList.ItemsSource = _orders;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }


    private void btnSow_Click(object sender, RoutedEventArgs e)
    {
        //NEWFUNC - Place a comprobation to open the sow input window because if I select an orderlocation
        //, close the rowdetail of the order and click the sowbutton it opens the window but it should
        //at least the row detail is open. And do the solution i came up with in the other similar methods
        CallSownOrderLocationSetter();
    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        CallSownOrderLocationSetter();
    }

    private void CallSownOrderLocationSetter()
    {
        if (_orderLocationInProcess == null)
        {
            MessageBox.Show("Debe seleccionar la locación que desea sembrar"
                , "", MessageBoxButton.OK, MessageBoxImage.Information);

            return;
        }

        SowInputWindow window = new SowInputWindow(this);
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

    public void SetTheSownOrderLocation(DateOnly date, short sownSeedTrays)
    {
        _orderLocationProcessor.SaveSownOrderLocationChange(_orderLocationInProcess, date, sownSeedTrays);

        _orderProcessor.UpdateOrderStatusAfterSow(_orderLocationInProcess.Order, date);

        RefreshTheDataGrids();
    }

    private void RefreshTheDataGrids()
    {
        if (_orderLocationInProcess.Order.Sown == true)
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
