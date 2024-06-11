using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presentation.Forms
{
    //Later - Set a background to the row detail template to show the information in a clearer way.

    //NEXT - Agregarle a este window un textBox para busqueda, un combobox para seleccionar casa y otro para bandeja
    // y un check para mostrar solo ordenes no ubicadas.

    /// <summary>
    /// Interaction logic for OrderDistributionWindow.xaml
    /// </summary>
    public partial class OrderDistributionWindow : Window
    {
        private ObservableCollection<Order> _orders;
        private OrderProcessor _orderProcessor;
        private Block _blockInProcess;
        private DataGrid _activeBlockDataGrid;
        public OrderDistributionWindow()
        {
            InitializeComponent();
            _orderProcessor = new OrderProcessor();
            LoadData();
        }

        private void LoadData()
        {
            _orders = new ObservableCollection<Order>(_orderProcessor.GetOrdersInTheSeedBed());

            foreach (Order order in _orders)
            {
                order.BlocksView = new ObservableCollection<Block>();

                foreach (OrderLocation orderLocation in order.OrderLocations)
                {
                    foreach (Block block in orderLocation.Blocks)
                    {
                        if (block.SeedTraysAmountToBeDelivered > 0)
                        {
                            order.BlocksView.Add(block);
                        }
                    }
                }
            }

            //CHECK - If is really needed to set the datacontex
            dgDistributionList.DataContext = this;
            dgDistributionList.ItemsSource = _orders;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRelocate_Click(object sender, RoutedEventArgs e)
        {
            CallRelocatedBlockSetter();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CallRelocatedBlockSetter();
        }

        private void CallRelocatedBlockSetter()
        {
            throw new NotImplementedException();
        }

        private void btnRowDetail_Click(object sender, RoutedEventArgs e)
        {
            //LATER - Maybe extract this logic to a static class
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

        public void SetTheRelocatedBlock()
        {
            RefreshTheDataGrids();
        }

        private void RefreshTheDataGrids()
        {
            throw new NotImplementedException();
        }

        public Block BlockInProcess { get => _blockInProcess; }
    }
}
