using Domain.Processors;
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
        //LATER - disable the row header in all the data grid that not need it.
        private List<GreenHouse> _greenHouses;
        private GreenHouseProcessor _processor;
        public GreenHousesWindow()
        {
            InitializeComponent();
            //CHECK - Evaluate if I remove this instanciation something would happen in the class
            _greenHouses = new List<GreenHouse>();
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
                    _processor.DeleteGreenHouse(greenHouse.Id);
                    _greenHouses.Remove(greenHouse);
                    RefreshData();
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
                MessageBox.Show("Debe seleccionar el registro que desea editar.");
            }
        }
    }
}
