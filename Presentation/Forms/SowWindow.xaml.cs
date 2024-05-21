using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.Forms;
//TODO - Extract the whole dg to a custom control because I have to reuse in other places. Tener en cuenta que 
//hay un evento que teng oque asignar que es el doble click en el dg hijo. Y tambien el cambio de algunas columnas 
//de los dg.

/// <summary>
/// Interaction logic for SowWindow.xaml
/// </summary>
public partial class SowWindow : Window
{
    private ObservableCollection<Order> _orders;
    private OrderProcessor _processor;

    public SowWindow()
    {
        InitializeComponent();
        _processor = new OrderProcessor();
        LoadData();
    }

    private void LoadData()
    {
        _orders = new ObservableCollection<Order>(_processor.GetNextOrdersToSow());
        dgDeliveryList.DataContext = this;
        dgDeliveryList.ItemsSource = _orders;
    }

    //NEXT - continue doing the logic of this window
    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }


    private void btnSow_Click(object sender, RoutedEventArgs e)
    {
        SetTheSownOrderLocation();
    }   

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        SetTheSownOrderLocation();
    }

    private void SetTheSownOrderLocation()
    {
        throw new NotImplementedException();
        //1-si el order location se sembro completo se le asigna la fecha real de siembra y el completo
        //2-si no, se crea un nuevo orderLocation y se le asigna lo que se sembró, la fecha y el completo 
        //3-se refresca el dg
    }

    private void btnRowDetail_Click(object sender, RoutedEventArgs e)
    {
        var row = DataGridRow.GetRowContainingElement((Button)sender);

        row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ?
        Visibility.Collapsed : Visibility.Visible;
    }


}
