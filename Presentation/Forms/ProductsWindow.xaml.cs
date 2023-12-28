using Domain.Processors;
using SupportLayer.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Presentation.Forms;

/// <summary>
/// Interaction logic for Products.xaml
/// </summary>
public partial class ProductsWindow : Window, ISpeciesRequester
{
    private ObservableCollection<Species> _species;
    private SpeciesProcessor _speciesProcessor;
    private ProductProcessor _productProcessor;
    private Product _productModel;

    public ProductsWindow()
    {
        InitializeComponent();
        _speciesProcessor = new SpeciesProcessor();
        _productProcessor = new ProductProcessor();
        _productModel = new Product();
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
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea editar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
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
            btnRemoveVariety.IsEnabled = true;
            txtNewVariety.Text = "";
        }
    }

    private void btnAddVariety_Click(object sender, RoutedEventArgs e)
    {
        //LATER - Create an string extension method in the SupportLayer project to check if a string is null or whitespace(this is whitespace"   ")

        ValidateDataType();

        if (_productModel.Id == 0)
        {
            ((Species)dgSpecies.SelectedItem).Products.Add(_productModel);
        }

        if (_productProcessor.SaveProduct(_productModel) == true)
        {
            RefreshListBox();

            _productModel = new Product();

            txtNewVariety.Text = "";
            btnAddVariety.Content = "Agregar";
            btnRemoveVariety.IsEnabled = true;
        }
        else
        {
            ShowError();
        }
    }

    private void ValidateDataType()
    {
        _productModel.Variety = txtNewVariety.Text;
        _productModel.SpecieId = ((Species)dgSpecies.SelectedItem).Id;
    }

    private void ShowError()
    {
        MessageBox.Show(_productProcessor.Error);
    }

    private void RefreshListBox()
    {
        Species species = (Species)dgSpecies.SelectedItem;
        lstVarieties.ItemsSource = null;
        lstVarieties.ItemsSource = species.Products.OrderBy(x => x.Variety);
    }

    private void btnRemoveVariety_Click(object sender, RoutedEventArgs e)
    {
        if (lstVarieties.SelectedItem is Product product)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?"
                , "Eliminar registro"
                , MessageBoxButton.YesNo
                , MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _productProcessor.DeleteProduct(product.Id);

                ((Species)dgSpecies.SelectedItem).Products.Remove(product);

                RefreshListBox();
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void lstVarieties_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        //LATER - In the database and all the logic above change the table Product by Variety
        if (lstVarieties.SelectedItem is Product product)
        {
            _productModel = product;
            txtNewVariety.Text = product.Variety;
            btnAddVariety.Content = "Editar";
            btnRemoveVariety.IsEnabled = false;
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea editar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
