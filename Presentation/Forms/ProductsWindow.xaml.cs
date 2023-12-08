using Domain.Processors;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Linq;
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
        _species = new ObservableCollection<Species>();
        _speciesProcessor = new SpeciesProcessor();
        LoadData();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    public void btnNewProduct_Click(object sender, RoutedEventArgs e)
    {
        AddEditSpeciesWindow window = new AddEditSpeciesWindow(this);
        window.ShowDialog();
    }

    private void btnEditProduct_Click(object sender, RoutedEventArgs e)
    {
        EditSpecies();
    }

    private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        EditSpecies();
    }

    private void EditSpecies()
    {
        if (dgProducts.SelectedItem is Species species)
        {
            AddEditSpeciesWindow window = new AddEditSpeciesWindow(species);
            window.ShowDialog();
            dgProducts.Items.Refresh();
        }
    }

    private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
    {
        if (dgProducts.SelectedItem is Species species)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _speciesProcessor.DeleteSpecies(species.Id);
                _species.Remove(species);
                //RefreshData();
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void RefreshData()
    {
        dgProducts.ItemsSource = null;
        dgProducts.ItemsSource = _species;
    }

    private void LoadData()
    {
        _species = new ObservableCollection<Species>(_speciesProcessor.GetAllSpecies());
        dgProducts.ItemsSource = null;
        dgProducts.DataContext = this;
        dgProducts.ItemsSource = _species;
    }

    public void SpeciesComplete(Species model)
    {
        //if (_species.FirstOrDefault(x => x.Id == model.Id, null) == null)
        //{
            _species.Add(model);
        //}
    }
}
