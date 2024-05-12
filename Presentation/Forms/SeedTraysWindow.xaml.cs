using Domain.Processors;
using SupportLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for SeedTrays.xaml
    /// </summary>
    public partial class SeedTraysWindow : Window
    {
        List<SeedTray> _seedTrays;
        SeedTrayProcessor _processor;
        public SeedTraysWindow()
        {
            InitializeComponent();
            _seedTrays = new List<SeedTray>();
            _processor = new SeedTrayProcessor();
            LoadData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNewSeedTray_Click(object sender, RoutedEventArgs e)
        {
            AddEditSeedTrayWindow window = new AddEditSeedTrayWindow((byte)(_seedTrays.Max(x => x.Preference) + 1));
            window.ShowDialog();
            LoadData();
        }

        private void btnEditSeedTray_Click(object sender, RoutedEventArgs e)
        {
            EditSeedTray();
        }

        private void dgSeedTrays_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSeedTray();
        }

        private void btnDeleteSeedTray_Click(object sender, RoutedEventArgs e)
        {
            if (dgSeedTrays.SelectedItem is SeedTray seedTray)
            {
                if (MessageBox.Show("Esta seguro que desea eliminar este registro?", "Eliminar registro"
                    , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _processor.DeleteSeedTray(seedTray.Id);
                    _seedTrays.Remove(seedTray);
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
            _seedTrays = _processor.GetAllSeedTrays().ToList();
            dgSeedTrays.ItemsSource = _seedTrays;
        }

        private void RefreshData()
        {
            dgSeedTrays.ItemsSource = null;
            dgSeedTrays.ItemsSource = _seedTrays;
        }

        private void EditSeedTray()
        {
            if (dgSeedTrays.SelectedItem is SeedTray seedTray)
            {
                AddEditSeedTrayWindow window = new AddEditSeedTrayWindow(seedTray);
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
