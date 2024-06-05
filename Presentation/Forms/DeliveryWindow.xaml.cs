using Domain.Processors;
using Presentation.InputForms;
using Presentation.IRequesters;
using SupportLayer.Models;
using System;
using System.Collections.ObjectModel;
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
    private BlockProcessor _blockProcessor;
    private Block _blockInProcess;
    private DataGrid _activeBlockDataGrid;

    public DeliveryWindow()
    {
        InitializeComponent();
        _blockProcessor = new BlockProcessor();
        LoadData();
    }

    private void LoadData()
    {
        _orders = new ObservableCollection<Order>(_orderProcessor.GetNextOrdersToDeliver());

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
        CallBlockDeliverSetter();
    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        CallBlockDeliverSetter();
    }

    private void CallBlockDeliverSetter()
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

    private void dgOrderLocationChild_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (sender is DataGrid datagrid)
        {
            _blockInProcess = (Block)datagrid.SelectedItem;
            _activeBlockDataGrid = datagrid;
        }
    }

    public void SetTheDeliveredBlock()
    {
        //NEXT - Create the method to save the delivers in the BlockProcessor.
        //NEXT - Create this method.

        RefreshTheDataGrids();
    }

    private void RefreshTheDataGrids()
    {
        //NEXT - Create the method to update the dg
        throw new NotImplementedException();
    }

    public Block BlockInProcess { get => _blockInProcess; }
}
