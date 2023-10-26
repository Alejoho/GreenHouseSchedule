using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for GreenHouses.xaml
    /// </summary>
    public partial class GreenHousesWindow : Window
    {
        List<GreenHouse> _greenHouses;
        public GreenHousesWindow()
        {
            InitializeComponent();
            _greenHouses = new List<GreenHouse>();
            LoadData();
        }

        private void LoadData()
        {
            _greenHouses.Add(new GreenHouse()
            {
                Id = 1,
                Name = "casa 1",
                Description = "es una casa grande",
                Width = 12,
                Length = 35,
                GreenHouseArea = 420,
                SeedTrayArea = 330,
                AmountOfBlocks = 4,
                Active = true
            });

            _greenHouses.Add(new GreenHouse()
            {
                Id = 2,
                Name = "casa 2",
                Description = "es una casa pequena",
                Width = 6,
                Length = 20,
                GreenHouseArea = 120,
                SeedTrayArea = 80,
                AmountOfBlocks = 2,
                Active = false
            });

            dgGreenHouses.ItemsSource = _greenHouses;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //NEXT - Do all the logic for showing all the houses, edit them and delete them
    }
}
