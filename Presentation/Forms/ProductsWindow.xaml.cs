using Domain.Processors;
using log4net;
using Presentation.AddEditForms;
using Presentation.IRequesters;
using Presentation.Resources;
using SupportLayer;
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
    private static readonly ILog _log = LogHelper.GetLogger();
    //NEWFUNC - Implement the logic of the use of "ObservableCollection" in all the others.
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

    private void btnDeleteSpecies_Click(object sender, RoutedEventArgs e)
    {
        if (dgSpecies.SelectedItem is Species species)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _speciesProcessor.DeleteSpecies(species.Id);
                    _species.Remove(species);

                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(species);
                    _log.Info("A Species record was deleted from the DB");
                    log4net.GlobalContext.Properties["Model"] = "";
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.IsForeignKeyViolation())
                {
                    _log.Error("Attend to delete a related Species record from the DB", ex);

                    MessageBox.Show("El registro seleccionado no se puede borrar, porque esta relacionado " +
                        "con otros registros.", "Operación Invalida", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    _log.Error("An unexpected database error occurred while deleting an organization.", ex);

                    MessageBox.Show("Ocurrió un error al intentar eliminar el registro.",
                        "Error de Base de Datos",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
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
        dgSpecies.ItemsSource = _species;
        lstProducts.DisplayMemberPath = "Variety";
    }

    public void SpeciesComplete(Species model)
    {
        _species.Add(model);
    }

    private void dgProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (dgSpecies.SelectedItem is Species species)
        {
            lstProducts.ItemsSource = species.Products.OrderBy(x => x.Variety);
            btnDeleteProduct.IsEnabled = true;
            txtNewProduct.Text = "";
        }
    }

    private void btnAddProduct_Click(object sender, RoutedEventArgs e)
    {
        if (ValidateDataType())
        {
            if (_productModel.Id == 0)
            {
                ((Species)dgSpecies.SelectedItem).Products.Add(_productModel);
            }

            if (_productProcessor.SaveProduct(_productModel) == true)
            {
                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(_productModel);
                _log.Info("A Product record was saved to the DB");
                log4net.GlobalContext.Properties["Model"] = "";

                RefreshListBox();
                _productModel = new Product();
                txtNewProduct.Text = "";
                btnAddProduct.Content = "Agregar";
                btnDeleteProduct.IsEnabled = true;
            }
            else
            {
                ShowError();
            }
        }
    }

    private bool ValidateDataType()
    {
        if (dgSpecies.SelectedItem == null)
        {
            MessageBox.Show("Debe seleccionar un cultivo antes de agregar una variedad"
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        _productModel.SpecieId = ((Species)dgSpecies.SelectedItem).Id;

        _productModel.Variety = txtNewProduct.Text;

        return true;
    }

    private void ShowError()
    {
        MessageBox.Show(_productProcessor.Error, "", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void RefreshListBox()
    {
        Species species = (Species)dgSpecies.SelectedItem;
        lstProducts.ItemsSource = null;
        lstProducts.ItemsSource = species.Products.OrderBy(x => x.Variety);
    }

    private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
    {
        if (lstProducts.SelectedItem is Product product)
        {
            if (MessageBox.Show("Esta seguro que desea eliminar este registro?"
                , "Eliminar registro"
                , MessageBoxButton.YesNo
                , MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _productProcessor.DeleteProduct(product.Id);

                    ((Species)dgSpecies.SelectedItem).Products.Remove(product);

                    RefreshListBox();

                    log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(product);
                    _log.Info("A Product record was deleted from the DB");
                    log4net.GlobalContext.Properties["Model"] = "";
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.IsForeignKeyViolation())
                {
                    _log.Error("Attend to delete a related Product record from the DB", ex);

                    MessageBox.Show("El registro seleccionado no se puede borrar, porque esta relacionado " +
                        "con otros registros.", "Operación Invalida", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    _log.Error("An unexpected database error occurred while deleting an organization.", ex);

                    MessageBox.Show("Ocurrió un error al intentar eliminar el registro.",
                        "Error de Base de Datos",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea eliminar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void lstProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (lstProducts.SelectedItem is Product product)
        {
            _productModel = product;
            txtNewProduct.Text = product.Variety;
            btnAddProduct.Content = "Editar";
            btnDeleteProduct.IsEnabled = false;

            log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(product);
            _log.Info("A doubleclick action was made on the lstProducts to edit a Product");
            log4net.GlobalContext.Properties["Model"] = "";
        }
        else
        {
            MessageBox.Show("Debe seleccionar el registro que desea editar."
                , "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
