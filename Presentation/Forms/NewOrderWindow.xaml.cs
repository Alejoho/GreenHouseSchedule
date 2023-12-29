using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
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
    private List<Client> _clients;
    private List<Product> _products;
    private List<SeedTray> _seedTrays;
    public NewOrderWindow()
    {
        InitializeComponent();
        _clientProcessor = new ClientProcessor();
        _productProcessor = new ProductProcessor();
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

        SeedTrayProcessor seedTrayProcessor = new SeedTrayProcessor();
        _seedTrays = seedTrayProcessor.GetActiveSeedTrays().ToList();
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

}
