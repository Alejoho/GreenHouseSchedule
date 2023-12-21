using Domain.Processors;
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

    public OrderListWindow()
    {
        InitializeComponent();
        _processor = new OrderProcessor();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {

    }
}
