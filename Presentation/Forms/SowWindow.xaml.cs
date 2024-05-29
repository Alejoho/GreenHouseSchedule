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
//TODO - Extract the whole dg to a custom control because I have to reuse in other places. Tener en cuenta que 
//hay un evento que teng oque asignar que es el doble click en el dg hijo. Y tambien el cambio de algunas columnas 
//de los dg.

/// <summary>
/// Interaction logic for SowWindow.xaml
/// </summary>
public partial class SowWindow : Window, IOrderLocationChangeRequester
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

    //NEXT - continue doing the logic of this window
    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }


    private void btnSow_Click(object sender, RoutedEventArgs e)
    {
        CallSownOrderLocationSetter();
    }   

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        CallSownOrderLocationSetter();
    }

    private void CallSownOrderLocationSetter()
    {
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
        
        _orderProcessor.UpdateOrderStatusAfterSow(_orderLocationInProcess.Order);

    }

    public OrderLocation OrderLocationInProcess { get => _orderLocationInProcess; }
}
