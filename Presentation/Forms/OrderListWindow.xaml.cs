﻿using Domain.Processors;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for OrderListWindow.xaml
/// </summary>
public partial class OrderListWindow : Window
{
    public ObservableCollection<Order> _orders;
    private OrderProcessor _processor;
    //NEXT - Load the data to the dgOrderList
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
}
