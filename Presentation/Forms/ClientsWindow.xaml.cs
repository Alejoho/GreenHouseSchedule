using Domain.Processors;
using Presentation.AddEditForms;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for Clients.xaml
/// </summary>
/// 
public partial class ClientsWindow : Window
{
    private List<Client> _clients;
    private ClientProcessor _processor;

    public ClientsWindow()
    {
        InitializeComponent();
        _clients = new List<Client>();
        _processor = new ClientProcessor();
        LoadData();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnNewClient_Click(object sender, RoutedEventArgs e)
    {
        AddEditClientWindow window = new AddEditClientWindow();
        window.ShowDialog();
        LoadData();
    }

    private void btnEditClient_Click(object sender, RoutedEventArgs e)
    {
        EditClient();
    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        EditClient();
    }

    //private void dgClients_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //{
    //    EditClient();
    //}

    private void btnDeleteClient_Click(object sender, RoutedEventArgs e)
    {
        if (dgClients.SelectedItem is Client client)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                    , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _processor.DeleteClient(client.Id);
                _clients.Remove(client);
                RefreshData();
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void lbltxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        string filter = lbltxtSearch.TextBox.Text;
        _clients = _processor.GetFilteredClients(filter).ToList();
        dgClients.ItemsSource = null;
        dgClients.ItemsSource = _clients;
    }

    private void LoadData()
    {
        _clients = _processor.GetAllClients().ToList();
        dgClients.ItemsSource = _clients;
    }

    private void RefreshData()
    {
        _clients = _processor.GetAllClients().ToList();
        dgClients.ItemsSource = null;
        dgClients.ItemsSource = _clients;
    }

    private void EditClient()
    {
        if (dgClients.SelectedItem is Client client)
        {
            AddEditClientWindow window = new AddEditClientWindow(client);
            window.ShowDialog();
            RefreshData();
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea editar.");
        }
    }
}
