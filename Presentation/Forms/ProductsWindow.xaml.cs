using Domain.Processors;
using Microsoft.IdentityModel.Tokens;
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
    private ObservableCollection<Species> _species;
    private SpeciesProcessor _speciesProcessor;
    private ProductProcessor _productProcessor;

    public ProductsWindow()
    {
        InitializeComponent();
        _species = new ObservableCollection<Species>();
        _speciesProcessor = new SpeciesProcessor();
        _productProcessor = new ProductProcessor();
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
        if (dgSpecies.SelectedItem is Species species)
        {
            AddEditSpeciesWindow window = new AddEditSpeciesWindow(species);
            window.ShowDialog();
            dgSpecies.Items.Refresh();
        }
    }

    //LATER - Review what I want to do when I delete a record with associate records in another table
    //Because in the database I have set DeleteRestric
    private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
    {
        if (dgSpecies.SelectedItem is Species species)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _speciesProcessor.DeleteSpecies(species.Id);
                _species.Remove(species);
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void LoadData()
    {
        _species = new ObservableCollection<Species>(_speciesProcessor.GetAllSpecies());
        dgSpecies.DataContext = this;
        dgSpecies.ItemsSource = _species;
        lstVarieties.DisplayMemberPath = "Variety";
    }

    public void SpeciesComplete(Species model)
    {
        _species.Add(model);
    }

    private void dgProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (dgSpecies.SelectedItem is Species species)
        {
            lstVarieties.ItemsSource = species.Products.OrderBy(x => x.Variety);            
        }
    }

    private void btnAddVariety_Click(object sender, RoutedEventArgs e)
    {
        //LATER - Create an string extension method in the SupportLayer project to check if a string is null or whitespace(this is whitespace"   ")
        if (txtNewVariety.Text != string.Empty && dgSpecies.SelectedItem is Species species)
        {
            Product newProduct = new Product();
            newProduct.SpecieId = species.Id;
            newProduct.Variety = txtNewVariety.Text;

            _productProcessor.SaveProduct(newProduct);

            species.Products.Add(newProduct);

            RefreshListBox();

            txtNewVariety.Text = "";
        }
    }

    private void RefreshListBox()
    {
            Species species = (Species)dgSpecies.SelectedItem;            
            lstVarieties.ItemsSource = null;
            lstVarieties.ItemsSource = species.Products.OrderBy(x => x.Variety);
    }

    private void btnRemoveVariety_Click(object sender, RoutedEventArgs e)
    {
            Species species = (Species)dgSpecies.SelectedItem;        
            Product product = (Product)lstVarieties.SelectedItem;
        
            _productProcessor.DeleteProduct(product.Id);

            species.Products.Remove(product);

            RefreshListBox();      
    }

    private void lstVarieties_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }


}
