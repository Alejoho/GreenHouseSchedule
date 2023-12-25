using Domain.Processors;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for OrderListWindow.xaml
/// </summary>
public partial class OrderListWindow : Window
{
    //LATER - Add HeadersVisibility="Column" to the datagrids across the board
    public ObservableCollection<Order> _orders;
    private OrderProcessor _processor;
    //NEXT - Create the row detail to show in each order their order locations
    public OrderListWindow()
    {
        InitializeComponent();
        _processor = new OrderProcessor();
        LoadData();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    //NEXT - Check the behavior of deleting an order  , bacause an order has many order detail records in the database
    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrderList.SelectedItem is Order order)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _processor.DeleteOrder(order.Id);
                _orders.Remove(order);
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void LoadData()
    {
        _orders = new ObservableCollection<Order>(_processor.GetAllOrders());
        dgOrderList.DataContext = this;
        dgOrderList.ItemsSource = _orders;
    }

    private void btnRowDetail_Click(object sender, RoutedEventArgs e)
    {
        var row = DataGridRow.GetRowContainingElement((Button)sender);

        row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ?
        Visibility.Collapsed : Visibility.Visible;
    }
}

public class DataContextHolderClass
{
    private ObservableCollection<Order> _orders;

    public ObservableCollection<Order> Orders { get; set; }
}
