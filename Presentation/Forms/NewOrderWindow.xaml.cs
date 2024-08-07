﻿using Domain;
using Domain.Models;
using Domain.Processors;
using Domain.ValuableObjects;
using log4net;
using Presentation.AddEditForms;
using Presentation.IRequesters;
using Presentation.Resources;
using SupportLayer;
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
    private static readonly ILog _log = LogHelper.GetLogger();
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
    private int _seedtrayPermutationID = 0;
    public int SeedtrayPermutationID
    {
        get { return ++_seedtrayPermutationID; }
    }

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

        _seedTrays = _seedTrayProcessor.GetActiveSeedTrays().ToList();
        _seedTrays.ForEach(x => x.IsSelected = x.Selected);
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
        lblcmbbtnClient.ComboBox.ItemsSource = _clients.OrderBy(x => x.Name);
        lblcmbbtnClient.ComboBox.SelectedItem = model;
    }

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

            OrderModel order = CreateTheNewOrder();

            _iterator = new DateIteratorAndResourceChecker(_status, order);

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(order);
            _log.Info("A SeedBedStatus and DateIteratorAndResourceChecker objects were created successfully");
            log4net.GlobalContext.Properties["Model"] = "";

            _iterator.LookForAvailability();

            if (_iterator.SeedTrayPermutations.Count > 0)
            {
                _log.Info("It was found disponibility for the new Order");

                permutationsToDisplay = new LinkedList<SeedTrayPermutation>(_iterator.SeedTrayPermutations);
                ToggleControls();
                DisplayResults();
            }
            else
            {
                _log.Info("It wasn't found disponibility for the new Order");

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
        btnSelectPermutation.IsEnabled = !_areControlsEnabled;

        btnSearchAvailability.Content =
            _areControlsEnabled == true ? "Buscar disponibilidad" : "Cambiar valores";

        if (_areControlsEnabled == true)
        {
            dgSeedTrayPermutations.ItemsSource = null;
            _seedtrayPermutationID = 0;
            dgOrderLocations.ItemsSource = null;
        }
    }

    private void DisplayResults()
    {
        dgSeedTrayPermutations.ItemsSource = null;
        dgSeedTrayPermutations.Items.Clear();
        dgSeedTrayPermutations.ItemsSource = permutationsToDisplay;
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
            ProductId = orderModel.Product.ID,
            AmountOfWishedSeedlings = int.Parse(txtAmountOfSeedlings.FieldContent),
            AmountOfAlgorithmSeedlings = orderModel.OrderLocations.Sum(x => x.SeedlingAmount),
            WishDate = DateOnly.FromDateTime((DateTime)dtpWishDate.TimePicker.SelectedDate),
            DateOfRequest = orderModel.RequestDate,
            EstimateSowDate = (DateOnly)orderModel.EstimateSowDate,
            EstimateDeliveryDate = (DateOnly)orderModel.EstimateDeliveryDate,
            Sown = false,
            Delivered = false
        };

        int orderLocationIndex = 1;

        foreach (OrderLocationModel orderLocationModel in orderModel.OrderLocations)
        {
            OrderLocation orderLocation = new OrderLocation()
            {
                Id = orderLocationIndex++,
                GreenHouseId = 0,
                SeedTrayId = Convert.ToByte(orderLocationModel.SeedTrayType),
                OrderId = 0,
                SeedTrayAmount = Convert.ToInt16(orderLocationModel.SeedTrayAmount),
                SeedlingAmount = orderLocationModel.SeedlingAmount,
                EstimateSowDate = orderLocationModel.SowDate,
                EstimateDeliveryDate = orderLocationModel.EstimateDeliveryDate,
                SeedTrayName = _seedTrays.First(x => x.Id == orderLocationModel.SeedTrayType).Name
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

        DateOnly estimateSowDate = DateOnly.FromDateTime((DateTime)dtpWishDate.TimePicker.SelectedDate);
        estimateSowDate = estimateSowDate.AddDays(-selectedProduct.Specie.ProductionDays);

        OrderModel output = new OrderModel(0
            , clientModel
            , productModel
            , int.Parse(txtAmountOfSeedlings.FieldContent)
            , new DateOnly().Today()
            , estimateSowDate
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
            MessageBox.Show("Debe seleccionar un cliente", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            lblcmbbtnProduct.ComboBox.Focus();
            return false;
        }

        if (lblcmbbtnProduct.ComboBox.SelectedItem == null)
        {
            MessageBox.Show("Debe seleccionar un producto", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            lblcmbbtnProduct.ComboBox.Focus();
            return false;
        }

        if (txtAmountOfSeedlings.FieldContent == null
            || txtAmountOfSeedlings.FieldContent == "")
        {
            MessageBox.Show("Debe especificar la cantidad de posturas", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            txtAmountOfSeedlings.TextBox.Focus();
            return false;
        }
        else if (int.TryParse(txtAmountOfSeedlings.FieldContent, out int amountOfSeedlings) == false)
        {
            MessageBox.Show("La cantidad de posturas no está en el formato correcto."
                , "Cantidad de posturas inválida"
                , MessageBoxButton.OK, MessageBoxImage.Warning);
            txtAmountOfSeedlings.TextBox.Focus();
            return false;
        }

        int daysToDeliver = ((Product)lblcmbbtnProduct.ComboBox.SelectedItem).Specie.ProductionDays;

        if (dtpWishDate.TimePicker.SelectedDate == null)
        {
            MessageBox.Show("Debe especificar la fecha en la que el cliente " +
                "desea las posturas.", "Dato faltante"
                , MessageBoxButton.OK, MessageBoxImage.Information);
            dtpWishDate.TimePicker.Focus();
            return false;
        }
        else if (dtpWishDate.TimePicker.SelectedDate.Value < DateTime.Today.AddDays(daysToDeliver))
        {
            MessageBox.Show("La fecha deseada debe ser más futura que el día presente, en tantos días " +
                "necesite el producto para estar listo.", "Fecha inválida"
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
            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(permutation);
            _log.Info("A SeedTrayPermutation was selected to display");
            log4net.GlobalContext.Properties["Model"] = "";

            SetTheResultingOrder(permutation);

            DisplayOrderLocations();
        }
        else
        {
            MessageBox.Show("Debe seleccionar una permutacion para mostrar las locaciones de la orden."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrderLocations.Items.Count > 0)
        {
            OrderProcessor processor = new OrderProcessor();

            foreach (OrderLocation orderLocation in _resultingOrder.OrderLocations)
            {
                orderLocation.Id = 0;
            }

            if (processor.SaveOrder(_resultingOrder) == true)
            {
                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_resultingOrder);
                _log.Info("An Order record and all its OrderLocations records were saved to the DB");
                log4net.GlobalContext.Properties["Model"] = "";

                MessageBox.Show("Nueva orden guardada");
                this.Close();
            }
            else
            {
                MessageBox.Show(processor.Error, "", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
        }
        else
        {
            MessageBox.Show("Faltan acciones por realizar para poder guardar la orden"
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
