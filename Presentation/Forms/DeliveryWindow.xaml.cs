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

/// <summary>
/// Interaction logic for DeliveryWindow.xaml
/// </summary>
public partial class DeliveryWindow : Window, IDeliveredBlockRequester
{
    private ObservableCollection<Order> _orders;
    private OrderProcessor _orderProcessor;
    private DeliveryDetailProcessor _deliveryDetailProcessor;
    private Block _blockInProcess;
    private DataGrid _activeBlockDataGrid;

    public DeliveryWindow()
    {
        InitializeComponent();
        _orderProcessor = new OrderProcessor();
        _deliveryDetailProcessor = new DeliveryDetailProcessor();
        LoadData();
    }

    private void LoadData()
    {
        _orders = new ObservableCollection<Order>(_orderProcessor.GetNextOrdersToDeliver());

        foreach (Order order in _orders)
        {
            order.BlocksView = new ObservableCollection<Block>();

            foreach (OrderLocation orderLocation in order.OrderLocations)
            {
                foreach (Block block in orderLocation.Blocks)
                {
                    int seedTraysAlreadyDelivered = block.DeliveryDetails.Sum(x => x.SeedTrayAmountDelivered);
                    int seedTraysToBeDelivered = block.SeedTrayAmount - seedTraysAlreadyDelivered;
                    if (seedTraysToBeDelivered > 0)
                    {
                        order.BlocksView.Add(block);
                    }
                }
            }
        }

        //CHECK - If is really needed to set the datacontex
        dgDeliveryList.DataContext = this;
        dgDeliveryList.ItemsSource = _orders;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnDeliver_Click(object sender, RoutedEventArgs e)
    {
        CallDeliveredBlockSetter();
    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        CallDeliveredBlockSetter();
    }

    private void CallDeliveredBlockSetter()
    {
        DeliverInputWindow window = new DeliverInputWindow(this);
        window.ShowDialog();
    }

    private void btnRowDetail_Click(object sender, RoutedEventArgs e)
    {
        var row = DataGridRow.GetRowContainingElement((Button)sender);

        row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ?
        Visibility.Collapsed : Visibility.Visible;
    }

    private void dgBlockChild_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is DataGrid datagrid)
        {
            _blockInProcess = (Block)datagrid.SelectedItem;
            _activeBlockDataGrid = datagrid;
        }
    }

    public void SetTheDeliveredBlock(DateOnly date, short deliveredSeedTrays)
    {
        _deliveryDetailProcessor.SaveNewDeliveryDetails(_blockInProcess, date, deliveredSeedTrays);

        _orderProcessor.UpdateOrderStatusAfterDelivery(_blockInProcess.OrderLocation.Order);

        RefreshTheDataGrids();
    }

    private void RefreshTheDataGrids()
    {
        Order order = _blockInProcess.OrderLocation.Order;

        if (_blockInProcess.SeedTraysAmountToBeDelivered == 0)
        {
            order.BlocksView.Remove(_blockInProcess);
        }

        if (order.BlocksView.Count == 0)
        {
            _orders.Remove(order);
        }

        _activeBlockDataGrid.Items.Refresh();
    }

    public Block BlockInProcess { get => _blockInProcess; }
}
