using Domain.Processors;
using SupportLayer.Models;
using System;
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
        List<GreenHouse> _greenHouses;
        GreenHouseProcessor _processor;
        public GreenHousesWindow()
        {
            InitializeComponent();
            _greenHouses = new List<GreenHouse>();
            _processor = new GreenHouseProcessor();
            LoadData();
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

        //NEXT - Create the logic to delete a record
        private void btnDeleteGreenHouse_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
