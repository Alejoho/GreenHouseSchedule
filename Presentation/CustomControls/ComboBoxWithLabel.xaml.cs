using System.Windows;
using System.Windows.Controls;

namespace Presentation.CustomControls
{
    /// <summary>
    /// Interaction logic for ComboBoxWithLabel.xaml
    /// </summary>
    public partial class ComboBoxWithLabel : UserControl
    {
        public event RoutedEventHandler ButtonClick;
        private string _fieldLabel;


        public ComboBoxWithLabel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string FieldLabel { get => _fieldLabel; set => _fieldLabel = value; }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick?.Invoke(sender, e);
        }

        public string ButtonTip { get; set; }
        public string ComboBoxTip { get; set; }
    }
}
