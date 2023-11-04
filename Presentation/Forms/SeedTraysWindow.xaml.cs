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

        }

        private void dgSeedTrays_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnDeleteSeedTray_Click(object sender, RoutedEventArgs e)
        {

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

        private void EditGreenHouse()
        {

        }

        //NEXT - Create all the logic of the SeedTraysWindows
    }
}
