using Domain.Processors;
using log4net;
using SupportLayer;
using SupportLayer.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for OrderListWindow.xaml
/// </summary>
public partial class OrderListWindow : Window
{

    //NEXT - hacer que en el dgOrderLocation child no se pueda seleccionar
    private OrderProcessor _processor;
    private Orders _orders;
    private CollectionViewSource _viewSource;

    public OrderListWindow()
    {
        InitializeComponent();
        _processor = new OrderProcessor();
        _orders = (Orders)this.Resources["orders"];
        _viewSource = (CollectionViewSource)this.Resources["cvsOrders"];
        LoadData();
    }

    private void LoadData()
    {
        var orders = _processor.GetAllOrders();

        foreach (var order in orders)
        {
            _orders.Add(order);
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrderList.SelectedItem is Order order)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ILog log = LogHelper.GetLogger();

                try
                {
                    _processor.DeleteOrder(order.Id);
                }
                catch (Exception ex)
                {
                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(order);
                    log.Error("There was an error deleting an Order record from the DB", ex);
                    log4net.GlobalContext.Properties["Model"] = "";

                    MessageBox.Show("Hubo un error borrando el registro seleccionado. La ventana va a cerrarse");

                    this.Close();
                }

                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(order);
                log.Info("An Order record and all its related records were deleted from the DB");
                log4net.GlobalContext.Properties["Model"] = "";

                _orders.Remove(order);
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void btnRowDetail_Click(object sender, RoutedEventArgs e)
    {
        var row = DataGridRow.GetRowContainingElement((Button)sender);

        row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ?
        Visibility.Collapsed : Visibility.Visible;
    }

    private void lbltxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewSource.View.Refresh();
    }

    private void CollectionViewSource_Filter(object sender, System.Windows.Data.FilterEventArgs e)
    {
        Order order = e.Item as Order;

        if (order != null)
        {
            e.Accepted = false;

            string filter = lbltxtSearch.TextBox.Text;
            string dateFormat = (string)Application.Current.Resources["DateFormat"];

            if (order.Id.ToString().Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.Client.Name.Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.Product.Specie.Name.Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.Product.Variety.Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.AmountOfWishedSeedlings.ToString().Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.AmountOfAlgorithmSeedlings.ToString().Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.DateOfRequest.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.WishDate.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.EstimateSowDate.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.EstimateDeliveryDate.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase))
            {
                e.Accepted = true;
            }

            if (order.RealSowDate.HasValue
                && order.RealSowDate.Value.ToString(dateFormat)
                .Contains(filter, System.StringComparison.CurrentCultureIgnoreCase))
            {
                e.Accepted = true;
            }

            if (order.RealDeliveryDate.HasValue
                && order.RealDeliveryDate.Value.ToString(dateFormat)
                .Contains(filter, System.StringComparison.CurrentCultureIgnoreCase))
            {
                e.Accepted = true;
            }
        }
    }
}