using Domain;
using Domain.Models;
using Domain.Processors;
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
    public NewOrderWindow()
    {
        InitializeComponent();
        _clientProcessor = new ClientProcessor();
        _productProcessor = new ProductProcessor();
        _seedTrayProcessor = new SeedTrayProcessor();
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
        dgSeedTraySelector.DataContext = this;
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
        if (ValidateData() == true)
        {
            _seedTrayProcessor.CheckChangeInTheSelection(_seedTrays);

            Client selectedClient = (Client)lblcmbbtnClient.ComboBox.SelectedItem;

            ClientModel clientModel = new ClientModel(selectedClient.Id
                , selectedClient.Name
                , selectedClient.NickName);

            Product selectedProduct = (Product)lblcmbbtnProduct.ComboBox.SelectedItem;

            ProductModel productModel = new ProductModel(selectedProduct.Id
                , selectedProduct.Variety
                , selectedProduct.Specie.Name
                , selectedProduct.Specie.ProductionDays);

            OrderModel newOrder = new OrderModel(0
                , clientModel
                , productModel
                , int.Parse(txtAmountOfSeedlings.FieldContent)
                , DateOnly.FromDateTime(DateTime.Now)
                , DateOnly.FromDateTime((DateTime)dtpWishDate.TimePicker.SelectedDate)
                , null
                , null
                , null
                , false);

            SeedBedStatus seedBed = new SeedBedStatus();
            DateIteratorAndResourceChecker iterator = 
                new DateIteratorAndResourceChecker(seedBed, newOrder);

            iterator.LookForAvailability();

            if(iterator.SeedTrayPermutations.Count>=0)
            {
                DisplayResults();
            }
            else
            {
                MessageBox.Show("No se encontro espacio para ubicar la nueva orden.");
            }
        }
    }

    private void DisplayResults()
    {
        //TODO - Do the logic of this method
        MessageBox.Show("Encontrado espacio para la nueva orden.");
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
}
