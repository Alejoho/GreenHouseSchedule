using Domain.Processors;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for Products.xaml
/// </summary>
public partial class ProductsWindow : Window
{
    public ObservableCollection<Species> _species;
    private SpeciesProcessor _speciesProcessor;

    public ProductsWindow()
    {
        InitializeComponent();
        //_species = new ObservableCollection<Species>();
        _speciesProcessor = new SpeciesProcessor();
        LoadData();
        //dgProducts.DataContext = this;
        //this.DataContext = this;        
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void btnNewProduct_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Diste click");
    }

    private void btnEditProduct_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
    {

    }

    private void LoadData()
    {
        _species = new ObservableCollection<Species>(_speciesProcessor.GetAllSpecies());
        //dgProducts.ItemsSource = _species;
    }
}
