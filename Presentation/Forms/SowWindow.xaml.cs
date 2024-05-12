using System.Windows;

namespace Presentation.Forms
{
    /// <summary>
    /// Interaction logic for SowWindow.xaml
    /// </summary>
    public partial class SowWindow : Window
    {
        public SowWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
