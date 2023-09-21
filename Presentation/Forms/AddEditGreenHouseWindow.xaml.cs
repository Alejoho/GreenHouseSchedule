using Domain;
using SupportLayer.DatabaseModels;
using System.Windows;


namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for GreenHousesWindow.xaml
    /// </summary>
    public partial class AddEditGreenHouseWindow : Window
    {
        private GreenHouseProcessor processor;
        private Greenhouse model;
        public AddEditGreenHouseWindow()
        {
            InitializeComponent();
            processor = new GreenHouseProcessor();
            model = new Greenhouse();
            model.ID = 0;
        }

        public AddEditGreenHouseWindow(Greenhouse model)
        {
            InitializeComponent();
            processor = new GreenHouseProcessor();
            this.model = model;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (processor.SaveGreenHouse(model) == true)
            {
                //TODO - mejorar el mensaje
                MessageBox.Show("Registro salvado");
                this.Close();
            }
            else
            {
                ShowError();
            }
        }

        private void ShowError()
        {
            MessageBox.Show(processor.FirstError);
        }
    }
}
