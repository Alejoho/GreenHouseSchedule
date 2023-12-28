using Domain.Processors;
using SupportLayer.Models;
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

    public void ProductComplete(Product model)
    {
        //TODO - Look up how to order the items of the combobox after adding a new item
        Product newProduct = _productProcessor.GetAProductById(model.Id);
        _products.Add(newProduct);        
        lblcmbbtnProduct.ComboBox.Items.Refresh();
        lblcmbbtnProduct.ComboBox.SelectedItem = newProduct;
    }
}
