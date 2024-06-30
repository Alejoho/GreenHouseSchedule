using Domain.Processors;
using Presentation.InputForms;
using Presentation.IRequesters;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Presentation.Forms;

//NEWFUNC - Falta la funcionalidad de poder aumentar o disminuir la cantidad de bandejas de un bloque.

//LATER - Set a background to the row detail template to show the information in a clearer way.

/// <summary>
/// Interaction logic for OrderDistributionWindow.xaml
/// </summary>
public partial class OrderDistributionWindow : Window, IRelocatedBlockRequester
{
    private ObservableCollection<Order> _orders;
    private CollectionViewSource _viewSource;
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

        _viewSource = new CollectionViewSource();
        _viewSource.Source = _orders;
        _viewSource.SortDescriptions.Add(new SortDescription("RealSowDate", ListSortDirection.Ascending));
        //LATER - maybe add other sorts. Like this one
        //_viewSource.SortDescriptions.Add(new SortDescription("Client.Name", ListSortDirection.Descending));
        dgDistributionList.ItemsSource = _viewSource.View;
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

        //checks if the a blockinprocess has 0 seedtrays and deleted from the blockview in their order.
        if (BlockInProcess.SeedTraysAmountToBeDelivered == 0)
        {
            BlockInProcess.OrderLocation.Order.BlocksView.Remove(BlockInProcess);
        }

        //refresh the dg.
        _activeBlockDataGrid.Items.Refresh();
    }

    public Block BlockInProcess { get => _blockInProcess; }

    private void lbltxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewSource.Filter += Filter;
    }

    private void Filter(object sender, FilterEventArgs e)
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
                || order.EstimateDeliveryDate.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.RealSowDate.Value.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
                || order.BlocksView.Any(MatchedAnyField))
            {
                e.Accepted = true;
            }
        }
    }

    private bool MatchedAnyField(Block block)
    {
        string filter = lbltxtSearch.TextBox.Text;
        string dateFormat = (string)Application.Current.Resources["DateFormat"];

        if (block.Id.ToString().Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.OrderLocation.GreenHouse.Name.Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.BlockName.Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.OrderLocation.SeedTray.Name.Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.SeedTraysAmountToBeDelivered.ToString().Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.SeedlingAmountToBeDelivered.ToString().Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.OrderLocation.EstimateSowDate.Value.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.OrderLocation.RealSowDate.Value.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase)
            || block.OrderLocation.EstimateDeliveryDate.Value.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if (block.OrderLocation.RealDeliveryDate.HasValue)
        {
            if (block.OrderLocation.RealDeliveryDate.Value.ToString(dateFormat).Contains(filter, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
