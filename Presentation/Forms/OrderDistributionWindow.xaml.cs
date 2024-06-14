using Domain.Processors;
using Presentation.InputForms;
using Presentation.IRequesters;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presentation.Forms;

//LATER - Look up how can I set a defaul ordenation for the dg, so when I add a new item itself
//go to its corresponding site

//LATER - Set a background to the row detail template to show the information in a clearer way.

//NEXT - Agregarle a este window un textBox para busqueda, un combobox para seleccionar casa y otro para bandeja
// y un check para mostrar solo ordenes no ubicadas.

/// <summary>
/// Interaction logic for OrderDistributionWindow.xaml
/// </summary>
public partial class OrderDistributionWindow : Window, IRelocatedBlockRequester
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
        RelocateInputWindow window = new RelocateInputWindow(this);
        window.ShowDialog();
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

    public void SetTheRelocatedBlock(byte greenHouse, byte block, short relocatedSeedTrays)
    {
        BlockProcessor processor = new BlockProcessor();

        processor.SaveRelocateBlockChange(BlockInProcess, greenHouse, block, relocatedSeedTrays);

        RefreshTheDataGrids();
    }

    private void RefreshTheDataGrids()
    {
        //find the new block and adds it to the BlockView.
        Order order = BlockInProcess.OrderLocation.Order;

        Block newBlock = (from location in order.OrderLocations
                          from block in location.Blocks
                          orderby block.Id descending
                          select block).First();

        order.BlocksView.Add(newBlock);

        //check if the a blockinprocess has 0 seedtrays and deleted from the blockview in their order.
        if (BlockInProcess.SeedTraysAmountToBeDelivered == 0)
        {
            BlockInProcess.OrderLocation.Order.BlocksView.Remove(BlockInProcess);
        }

        //refresh the dg.
        _activeBlockDataGrid.Items.Refresh();
    }

    public Block BlockInProcess { get => _blockInProcess; }
}
