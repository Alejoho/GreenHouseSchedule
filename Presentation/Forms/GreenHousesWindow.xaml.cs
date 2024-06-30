using Domain.Processors;
using log4net;
using Presentation.AddEditForms;
using SupportLayer;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for GreenHouses.xaml
    /// </summary>
    public partial class GreenHousesWindow : Window
    {
        private List<GreenHouse> _greenHouses;
        private GreenHouseProcessor _processor;
        public GreenHousesWindow()
        {
            InitializeComponent();
            _processor = new GreenHouseProcessor();
            LoadData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNewGreenHouse_Click(object sender, RoutedEventArgs e)
        {
            AddEditGreenHouseWindow window = new AddEditGreenHouseWindow();
            window.ShowDialog();
            LoadData();
        }

        private void btnEditGreenHouse_Click(object sender, RoutedEventArgs e)
        {
            EditGreenHouse();
        }

        private void dgGreenHouses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditGreenHouse();
        }

        private void btnDeleteGreenHouse_Click(object sender, RoutedEventArgs e)
        {
            if (dgGreenHouses.SelectedItem is GreenHouse greenHouse)
            {
                if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                    , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ILog log = LogHelper.GetLogger();

                    try
                    {
                        _processor.DeleteGreenHouse(greenHouse.Id);
                        _greenHouses.Remove(greenHouse);
                        RefreshData();

                        log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(greenHouse);
                        log.Info("A GreenHouse record was deleted from the DB");
                        log4net.GlobalContext.Properties["Model"] = "";
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                    {
                        log.Error("Attend to delete a related GreenHouse record from the DB", ex);

                        MessageBox.Show("El registro seleccionado no se puede borrar, porque esta relacionado " +
                            "con otros registros.", "Operación Invalida", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            _greenHouses = _processor.GetAllGreenHouses().ToList();
            dgGreenHouses.ItemsSource = _greenHouses;
        }

        private void RefreshData()
        {
            dgGreenHouses.ItemsSource = null;
            dgGreenHouses.ItemsSource = _greenHouses;
        }

        private void EditGreenHouse()
        {
            if (dgGreenHouses.SelectedItem is GreenHouse greenHouse)
            {

                AddEditGreenHouseWindow window = new AddEditGreenHouseWindow(greenHouse);
                window.ShowDialog();
                RefreshData();
            }
            else
            {
                MessageBox.Show("Debe seleccionar el registro que desea editar."
                    , "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
