using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for Products.xaml
/// </summary>
public partial class ProductsWindow : Window, ISpeciesRequestor
{
    public ObservableCollection<Species> _species;
    private SpeciesProcessor _speciesProcessor;

    public ProductsWindow()
    {
        InitializeComponent();
        //_species = new ObservableCollection<Species>();
        _speciesProcessor = new SpeciesProcessor();
        LoadData();       
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void btnNewProduct_Click(object sender, RoutedEventArgs e)
    {
        AddEditSpeciesWindow  window = new AddEditSpeciesWindow();
        window.ShowDialog();
        //LoadData();
    }

    private void btnEditProduct_Click(object sender, RoutedEventArgs e)
    {

    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        EditSpecies();
    }

    private void dgProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        //MessageBox.Show("You clicked the DataGrid itself");
        //EditSpecies();
    }

    private void EditSpecies()
    {
        if (dgProducts.SelectedItem is Species species)
        {
            AddEditSpeciesWindow window = new AddEditSpeciesWindow(species);
            window.ShowDialog();
            dgProducts.Items.Refresh();
            //RefreshData();
        }
        //else
        //{
        //    MessageBox.Show("Debe seleccionar el registro que desea editar.");
        //}
    }

    private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
    {

    }

    private void LoadData()
    {
        _species = new ObservableCollection<Species>(_speciesProcessor.GetAllSpecies());
        dgProducts.ItemsSource = _species;
    }

    public void SpeciesComplete()
    {
        throw new NotImplementedException();
    }
}
