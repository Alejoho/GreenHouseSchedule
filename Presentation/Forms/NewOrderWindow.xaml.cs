using Domain;
using Domain.Models;
using Domain.Processors;
using Domain.ValuableObjects;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for NewOrderWindow.xaml
/// </summary>
public partial class NewOrderWindow : Window, IClientRequester, IProductRequester
{
    private ClientProcessor _clientProcessor;
    private ProductProcessor _productProcessor;
    private SeedTrayProcessor _seedTrayProcessor;
    private List<Client> _clients;
    private List<Product> _products;
    private List<SeedTray> _seedTrays;
    private SeedBedStatus _status;
    private DateIteratorAndResourceChecker _iterator;
    private LinkedList<SeedTrayPermutation> permutationsToDisplay;
    private Order _resultingOrder;
    private bool _areControlsEnabled;
    public NewOrderWindow()
    {
        InitializeComponent();
        _clientProcessor = new ClientProcessor();
        _productProcessor = new ProductProcessor();
        _seedTrayProcessor = new SeedTrayProcessor();
        _areControlsEnabled = true;
        LoadData();
    }

    private void LoadData()
    {
        _clients = _clientProcessor.GetAllClients().ToList();
        lblcmbbtnClient.ComboBox.ItemsSource = _clients;
        lblcmbbtnClient.ComboBox.DisplayMemberPath = "Name";

        _products = _productProcessor.GetAllProducts().ToList();
        lblcmbbtnProduct.ComboBox.ItemsSource = _products;
        lblcmbbtnProduct.ComboBox.DisplayMemberPath = "SpeciesAndVariety";

        //----------------------------------------------------------------

        _seedTrays = _seedTrayProcessor.GetActiveSeedTrays().ToList();
        _seedTrays.ForEach(x => x.IsSelected = x.Selected);
        //dgSeedTraySelector.DataContext = this;
        dgSeedTraySelector.ItemsSource = _seedTrays;

    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void lblcmbbtnClient_ButtonClick(object sender, RoutedEventArgs e)
    {
        AddEditClientWindow window = new AddEditClientWindow(this);
        window.ShowDialog();
    }

    private void lblcmbbtnProduct_ButtonClick(object sender, RoutedEventArgs e)
    {
        AddProductWindow window = new AddProductWindow(this);
        window.ShowDialog();
    }

    public void ClientComplete(Client model)
    {
        _clients.Add(model);
        lblcmbbtnClient.ComboBox.Items.Refresh();
        lblcmbbtnClient.ComboBox.SelectedItem = model;
    }
    //CHECK - that all of the combobox with button order their items when a new item is inserted
    public void ProductComplete(Product model)
    {
        Product newProduct = _productProcessor.GetAProductById(model.Id);
        _products.Add(newProduct);
        lblcmbbtnProduct.ComboBox.ItemsSource = _products.OrderBy(x => x.SpeciesAndVariety);
        lblcmbbtnProduct.ComboBox.SelectedItem = newProduct;
    }

    private void btnSearchAvailability_Click(object sender, RoutedEventArgs e)
    {
        if (_areControlsEnabled == true && ValidateData() == true)
        {
            _seedTrayProcessor.CheckChangeInTheSelection(_seedTrays);

            _status = new SeedBedStatus();

            _iterator = new DateIteratorAndResourceChecker(_status, CreateTheNewOrder());

            _iterator.LookForAvailability();

            if (_iterator.SeedTrayPermutations.Count > 0)
            {
                permutationsToDisplay = new LinkedList<SeedTrayPermutation>(_iterator.SeedTrayPermutations);
                ToggleControls();
                DisplayResults();
            }
            else
            {
                MessageBox.Show("No se encontró espacio para ubicar la nueva orden.");
            }
        }
        else if (_areControlsEnabled == false)
        {
            ToggleControls();
        }
            }

    private void ToggleControls()
    {
        _areControlsEnabled = !_areControlsEnabled;

        lblcmbbtnClient.IsEnabled = _areControlsEnabled;
        lblcmbbtnProduct.IsEnabled = _areControlsEnabled;
        txtAmountOfSeedlings.IsEnabled = _areControlsEnabled;
        dtpWishDate.IsEnabled = _areControlsEnabled;
        dgSeedTraySelector.IsEnabled = _areControlsEnabled;

        btnSearchAvailability.Content =
            _areControlsEnabled == true ? "Buscar disponibilidad" : "Cambiar valores";

        if (_areControlsEnabled == true)
        {
            dgSeedTrayPermutations.ItemsSource = null;
            dgOrderLocations.ItemsSource = null;
        }
    }

    private void DisplayResults()
    {
        //TODO - Look how to put and id to the permutation but without putting it in the class.

        //dgSeedTrayPermutations.DataContext = this;

        dgSeedTrayPermutations.Items.Clear();
        dgSeedTrayPermutations.ItemsSource = null;
        dgSeedTrayPermutations.ItemsSource = permutationsToDisplay;
        dgSeedTrayPermutations.SelectedItem = dgSeedTrayPermutations.Items[0];

        SetTheResultingOrder((SeedTrayPermutation)dgSeedTrayPermutations.Items[0]);

        DisplayOrderLocations();
    }

    private void DisplayOrderLocations()
    {
        dgOrderLocations.ItemsSource = null;
        dgOrderLocations.ItemsSource = _resultingOrder.OrderLocations.OrderBy(x => x.EstimateSowDate);
    }

    private void SetTheResultingOrder(SeedTrayPermutation permutation)
    {
        OrderModel orderModel = _iterator.GenerateOrderModel(permutation, _status);

        Order newOrder = new Order()
        {
            Id = 0,
            ClientId = orderModel.Client.ID,
            //CHECK - If this ProductId refers to the variety of the specie
            ProductId = orderModel.Product.ID,
            AmountOfWishedSeedlings = int.Parse(txtAmountOfSeedlings.FieldContent),
            AmountOfAlgorithmSeedlings = orderModel.SeedlingAmount,
            WishDate = DateOnly.FromDateTime((DateTime)dtpWishDate.TimePicker.SelectedDate),
            DateOfRequest = orderModel.RequestDate,
            EstimateSowDate = (DateOnly)orderModel.EstimateSowDate,
            EstimateDeliveryDate = (DateOnly)orderModel.EstimateDeliveryDate,
            Complete = false
        };

        foreach (OrderLocationModel orderLocationModel in orderModel.OrderLocations)
        {
            OrderLocation orderLocation = new OrderLocation()
            {
                Id = 0,
                GreenHouseId = 0,
                //LATER - Maybe change this type from int to byte
                SeedTrayId = Convert.ToByte(orderLocationModel.SeedTrayType),
                OrderId = 0,
                //LATER - Maybe change this type from int to short
                SeedTrayAmount = Convert.ToInt16(orderLocationModel.SeedTrayAmount),
                SeedlingAmount = orderLocationModel.SeedlingAmount,
                EstimateSowDate = orderLocationModel.SowDate,
                EstimateDeliveryDate = orderLocationModel.EstimateDeliveryDate,
            };

            newOrder.OrderLocations.Add(orderLocation);
        }

        _resultingOrder = newOrder;
    }

    private OrderModel CreateTheNewOrder()
    {
        Client selectedClient = (Client)lblcmbbtnClient.ComboBox.SelectedItem;

        ClientModel clientModel = new ClientModel(selectedClient.Id
            , selectedClient.Name
            , selectedClient.NickName);

        Product selectedProduct = (Product)lblcmbbtnProduct.ComboBox.SelectedItem;

        ProductModel productModel = new ProductModel(selectedProduct.Id
            , selectedProduct.Variety
            , selectedProduct.Specie.Name
            , selectedProduct.Specie.ProductionDays);

        OrderModel output = new OrderModel(0
            , clientModel
            , productModel
            , int.Parse(txtAmountOfSeedlings.FieldContent)
            , DateOnly.FromDateTime(DateTime.Now)
            , DateOnly.FromDateTime((DateTime)dtpWishDate.TimePicker.SelectedDate)
            , null
            , null
            , null
            , false);

        return output;
    }

    private bool ValidateData()
    {
        if (lblcmbbtnClient.ComboBox.SelectedItem == null)
        {
            MessageBox.Show("Debe seleccionar un cliente.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            lblcmbbtnProduct.ComboBox.Focus();
            return false;
        }

        if (lblcmbbtnProduct.ComboBox.SelectedItem == null)
        {
            MessageBox.Show("Debe seleccionar un producto.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            lblcmbbtnProduct.ComboBox.Focus();
            return false;
        }

        if (txtAmountOfSeedlings.FieldContent == null
            || txtAmountOfSeedlings.FieldContent == "")
        {
            MessageBox.Show("Debe especificar la cantidad de posturas.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            txtAmountOfSeedlings.TextBox.Focus();
            return false;
        }
        else if (int.TryParse(txtAmountOfSeedlings.FieldContent, out int amountOfSeedlings) == false)
        {
            MessageBox.Show("La cantidad de posturas no esta en el formato correcto."
                , "Cantidad de posturas inválida"
                , MessageBoxButton.OK, MessageBoxImage.Warning);
            txtAmountOfSeedlings.TextBox.Focus();
            return false;
        }

        if (dtpWishDate.TimePicker.SelectedDate == null)
        {
            MessageBox.Show("Debe especificar la fecha en la que el cliente " +
                "desea las posturas.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            dtpWishDate.TimePicker.Focus();
            return false;
        }
        else if (dtpWishDate.TimePicker.SelectedDate.Value < DateTime.Now)
        {
            MessageBox.Show("La fecha especificada es mas antigua que el dia presente."
                , "Fecha inválida"
            , MessageBoxButton.OK, MessageBoxImage.Warning);
            dtpWishDate.TimePicker.Focus();
            return false;
        }

        bool isAnySelected = false;

        foreach (SeedTray item in dgSeedTraySelector.Items)
        {
            if (item.IsSelected == true)
            {
                isAnySelected = true;
            }
        }

        if (isAnySelected == false)
        {
            MessageBox.Show("Debe seleccionar al menos un tipo de bandeja."
                , "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        return true;
    }

    private void btnSelectPermutation_Click(object sender, RoutedEventArgs e)
    {
        if (dgSeedTrayPermutations.SelectedItem is SeedTrayPermutation permutation)
        {
            SetTheResultingOrder(permutation);

            DisplayOrderLocations();
        }
        else
        {
            MessageBox.Show("Debe seleccionar una permutacion para mostrar las locaciones de la orden.");
        }
    }
}
